using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nicodem.Semantics.AST;

namespace Nicodem.Semantics.Visitors
{
    class TypeCheckVisitor: AbstractRecursiveVisitor
    {
        List<WhileNode> _stack_of_while_node = new List<WhileNode>();

        //check if it is array type
        public override void Visit(ArrayNode node)
        {
            base.Visit(node);
            var typeOfArray = node.VariableType;
            if(typeOfArray == null)
                throw new TypeCheckException("No set Type for Array.");
			if (!(typeOfArray is ArrayTypeNode))
                throw new TypeCheckException("Array doesn't have array type"); 
            var typeOfElementInArray = ((ArrayTypeNode)typeOfArray).ElementType;
            if (typeOfElementInArray == null)
                throw new TypeCheckException("Type of element in array is not set.");
            foreach (var expression in node.Elements)
            {
                if (expression.ExpressionType == null)
                    throw new TypeCheckException("Array contains value with not specified type.");
                if (!typeOfElementInArray.Equals(expression.ExpressionType))
                    throw new TypeCheckException("Improper Type of element in array.");
            }
            node.ExpressionType = node.VariableType;
        }

        private HashSet<TypeNode> deduceType(string value)
        {
            HashSet<TypeNode> _set_of_types = new HashSet<TypeNode>();
            bool out_bool;
            if (Boolean.TryParse(value, out out_bool))
                _set_of_types.Add(NamedTypeNode.BoolType());
            byte out_byte;
            if (Byte.TryParse(value, out out_byte))
                _set_of_types.Add(NamedTypeNode.ByteType());
            int out_int;
			if (Int32.TryParse (value, out out_int)) {
				_set_of_types.Add (NamedTypeNode.IntType ());
			}
            char out_char;
            if (value.Length == 3 && Char.TryParse(value.Trim(new char[]{'\''}), out out_char))
                _set_of_types.Add(NamedTypeNode.CharType());
            return _set_of_types;
        }

        //try to find definition type on base of Value
        public override void Visit(AtomNode node)
		{
			base.Visit (node);
			if (node.Value == null)
				throw new TypeCheckException ("Value is not set.");

            HashSet<TypeNode> deducedType = deduceType(node.Value);
			if (!deducedType.Contains(node.VariableType))
				throw new TypeCheckException ("Type of Const is not consistent with deduced Type.");

			node.ExpressionType = node.VariableType;
        }

        //type is the last one element, if don't have set void
        public override void Visit(BlockExpressionNode node)
        {
            base.Visit(node);
            TypeNode last_element_type = NamedTypeNode.VoidType();
            foreach (var element in node.Elements)
                last_element_type = element.ExpressionType;
            node.ExpressionType = last_element_type;
        }

        //check if number is int and rewrite ExpressionType to Array type
        public override void Visit(ElementNode node)
        {
            base.Visit(node);
            if (!NamedTypeNode.IntType().Equals(node.Index.ExpressionType))
                throw new TypeCheckException("Index is not integer type.");
            node.ExpressionType = node.Array.ExpressionType;
        }

        //check if function resoluted has the same arguments type and return type 
        public override void Visit(FunctionCallNode node)
        {
            base.Visit(node);
            
            if (node.Definition == null)
                throw new TypeCheckException("Definition is not resolved.");
            
            var listOfFunctionCallArguments = node.Arguments.ToList();
            var listOfFunctionArguments = node.Definition.Parameters.ToList();
            
            if(listOfFunctionArguments.Count != listOfFunctionCallArguments.Count)
                throw new TypeCheckException("Improper number of arguments.");
            
            for (int i = 0; i < listOfFunctionArguments.Count; i++)
            {
                if (!listOfFunctionArguments[i].Type.Equals(listOfFunctionCallArguments[i].ExpressionType))
                    throw new Exception(String.Format("Argument {0} has improper type.", i));
            }

            node.ExpressionType = node.Definition.ResultType;
        }

        //check if body returns the same as set type
        public override void Visit(FunctionDefinitionNode node)
        {
            base.Visit(node);
			if (!node.ResultType.Equals(node.Body.ExpressionType))
				throw new TypeCheckException ("Body doesn't return the same type as set type.");
            node.ExpressionType = NamedTypeNode.VoidType();
        }

        //check if condition is bool and set common return type of Then and Else
        public override void Visit(IfNode node)
        {
            base.Visit(node);
            if(!NamedTypeNode.BoolType().Equals(node.Condition.ExpressionType))
                throw new Exception("Improper type in if condition");
            if(!node.HasElse || !node.Then.ExpressionType.Equals(node.Else.ExpressionType))
                node.ExpressionType = NamedTypeNode.VoidType();
            else
                node.ExpressionType = node.Then.ExpressionType;
        }

        //check if while return type is the same as in loopControlNode return type
        public override void Visit(LoopControlNode node)
        {
            base.Visit(node);
            //get proper while
            var length = _stack_of_while_node.Count;
            if (node.Depth > length)
                throw new TypeCheckException("Improper depth.");
            var while_elem = _stack_of_while_node[length - node.Depth];
            if (while_elem.ExpressionType == null)
                while_elem.ExpressionType = node.Value.ExpressionType;
            else
                if (!while_elem.ExpressionType.Equals(node.Value.ExpressionType))
                    throw new TypeCheckException("Value Type is not correct for returned while type.");
            node.ExpressionType = node.Value.ExpressionType;
        }

        //for every pair of arguments type should be proper types
        public override void Visit(OperatorNode node)
        {
            base.Visit(node);
            List<ExpressionNode> arguments_list = new List<ExpressionNode>(node.Arguments);
            switch (node.Operator)
            {
                // =
                case OperatorType.ASSIGN:
                    if (arguments_list.Count() != 2)
                        throw new TypeCheckException("Inproper numbers of arguments.");
                    if (!arguments_list[0].ExpressionType.Equals(arguments_list[1].ExpressionType))
                        throw new TypeCheckException("Wrong argument type.");
                    node.ExpressionType = arguments_list[0].ExpressionType;
                    break;
                // + - * / % += -= *= /= %= 
                case OperatorType.PLUS:
                case OperatorType.MINUS:
                case OperatorType.MUL:
                case OperatorType.DIV:
                case OperatorType.MOD:
                case OperatorType.PLUS_ASSIGN:
                case OperatorType.MINUS_ASSIGN:
                case OperatorType.MUL_ASSIGN:
                case OperatorType.DIV_ASSIGN:
                case OperatorType.MOD_ASSIGN:
                    if (arguments_list.Count() != 2)
                        throw new TypeCheckException("Inproper numbers of arguments.");
                    if (NamedTypeNode.IntType().Equals(arguments_list[0].ExpressionType) && NamedTypeNode.IntType().Equals(arguments_list[1].ExpressionType))
                        node.ExpressionType = NamedTypeNode.IntType();
                    else if ( NamedTypeNode.ByteType().Equals(arguments_list[0].ExpressionType) && NamedTypeNode.ByteType().Equals(arguments_list[1].ExpressionType))
                        node.ExpressionType = NamedTypeNode.IntType();
                    else
                        throw new TypeCheckException("Wrong argument type.");
                    break;
                // << >> <<= >>= 
                case OperatorType.BIT_SHIFT_UP:
                case OperatorType.BIT_SHIFT_DOWN:
                case OperatorType.BIT_SHIFT_UP_ASSIGN:
                case OperatorType.BIT_SHIFT_DOWN_ASSIGN:
                    if (arguments_list.Count() != 2)
                        throw new TypeCheckException("Inproper numbers of arguments.");
                    if (!NamedTypeNode.ByteType().Equals(arguments_list[0].ExpressionType) || !NamedTypeNode.IntType().Equals(arguments_list[1].ExpressionType) )
                        throw new TypeCheckException("Wrong argument type.");
                    node.ExpressionType = NamedTypeNode.ByteType();
                    break;
                //| ^ & 
                case OperatorType.BIT_OR:
                case OperatorType.BIT_XOR:
                case OperatorType.BIT_AND:
                    foreach (var argument in node.Arguments)
                        if (!NamedTypeNode.ByteType().Equals(argument.ExpressionType))
                            throw new TypeCheckException("Wrong argument type.");
                    node.ExpressionType = NamedTypeNode.ByteType();
                    break;
                //&= ^= |=
                case OperatorType.BIT_AND_ASSIGN:
                case OperatorType.BIT_XOR_ASSIGN:
                case OperatorType.BIT_OR_ASSIGN:
                    if (arguments_list.Count() != 2)
                        throw new TypeCheckException("Inproper numbers of arguments.");
                    if (!NamedTypeNode.ByteType().Equals(arguments_list[0].ExpressionType) || !NamedTypeNode.ByteType().Equals(arguments_list[1].ExpressionType))
                        throw new TypeCheckException("Wrong argument type.");
                    node.ExpressionType = NamedTypeNode.ByteType();
                    break;
                // || && 
                case OperatorType.OR:
                case OperatorType.AND:
                    foreach (var argument in node.Arguments)
                        if (!NamedTypeNode.BoolType().Equals(argument.ExpressionType))
                            throw new TypeCheckException("Wrong argument type.");
                    node.ExpressionType = NamedTypeNode.BoolType();
                    break;
                // ==!= < <= > >=
				case OperatorType.EQUAL:
				case OperatorType.NOT_EQUAL:
				case OperatorType.LESS:
				case OperatorType.LESS_EQUAL:
				case OperatorType.GREATER:
				case OperatorType.GREATER_EQUAL:
					if (arguments_list.Count () != 2)
						throw new TypeCheckException ("Inproper numbers of arguments.");
					if (!NamedTypeNode.IntType ().Equals (arguments_list [0].ExpressionType) || !NamedTypeNode.IntType ().Equals (arguments_list [1].ExpressionType)) {
						Console.WriteLine ("ERROR: First arg type =  {" + arguments_list [0].ExpressionType + "} Second arg type = {" + arguments_list [1].ExpressionType + "}");    
						throw new TypeCheckException ("Wrong argument type.");
					}
                    node.ExpressionType = NamedTypeNode.BoolType();
                    break;
                // PRE ++ -- + - POST ++ --
                case OperatorType.PRE_INCREMENT:
                case OperatorType.PRE_DECREMENT:
                case OperatorType.UNARY_PLUS:
                case OperatorType.UNARY_MINUS:
                case OperatorType.POST_INCREMENT:
                case OperatorType.POST_DECREMENT:
                    if (arguments_list.Count() != 1)
                        throw new TypeCheckException("Inproper numbers of arguments.");
                    if (NamedTypeNode.IntType().Equals(arguments_list[0].ExpressionType))
                        node.ExpressionType = NamedTypeNode.IntType();
                    else if (NamedTypeNode.ByteType().Equals(arguments_list[0].ExpressionType))
                        node.ExpressionType = NamedTypeNode.ByteType();
                    else
                        throw new TypeCheckException("Wrong argument type.");
                    break;
                // PRE ! ~
                case OperatorType.NOT:
                case OperatorType.BIT_NOT:
                    if (arguments_list.Count() != 1)
                        throw new TypeCheckException("Inproper numbers of arguments.");
                    if (!NamedTypeNode.ByteType().Equals(arguments_list[0].ExpressionType))
                        throw new TypeCheckException("Wrong argument type.");
                    node.ExpressionType = NamedTypeNode.ByteType();
                    break;
            }
        }

        //write type the same as in array node and check if Left and Right have int type
        public override void Visit(SliceNode node)
        {
            base.Visit(node);
            if (!NamedTypeNode.IntType().Equals(node.Left.ExpressionType))
                throw new TypeCheckException("Left is not int value.");
            if (!NamedTypeNode.IntType().Equals(node.Right.ExpressionType))
                throw new TypeCheckException("Right is not int value.");
            node.ExpressionType = node.Array.ExpressionType;
            ((ArrayTypeNode)node.ExpressionType).IsFixedSize = false;
        }

        public override void Visit(VariableDeclNode node)
        {
            base.Visit(node);
            node.ExpressionType = node.Type;
        }

        //value has the same type as definition and it is rewrite to my type
        public override void Visit(VariableDefNode node)
        {
            base.Visit(node);
            if (!node.Value.ExpressionType.Equals(node.Type))
                throw new TypeCheckException("Value type not agree with VariableType");
            node.ExpressionType = node.Type;
        }

        //rewerite type
        public override void Visit(VariableUseNode node)
        {
            base.Visit(node);
            node.ExpressionType = node.Declaration.ExpressionType;
        }

        //check condtion type and set returned type as in if
        public override void Visit(WhileNode node)
        {
            _stack_of_while_node.Add(node);
            base.Visit(node);
            _stack_of_while_node.Remove(node);
            TypeNode _type_to_set;
            if(!NamedTypeNode.BoolType().Equals(node.Condition.ExpressionType))
                throw new Exception("Inpropper type in if condition");
            if(!node.HasElse || !node.Body.ExpressionType.Equals(node.Else.ExpressionType))
                _type_to_set = NamedTypeNode.VoidType();
            else
                _type_to_set = node.Body.ExpressionType;
            if (node.ExpressionType == null)
                node.ExpressionType = _type_to_set;
            else
                if (!node.ExpressionType.Equals(_type_to_set))
                    throw new Exception("Type of Body and Else is not correct with set type.");
                else
                    node.ExpressionType = _type_to_set;
        }
    }
}
