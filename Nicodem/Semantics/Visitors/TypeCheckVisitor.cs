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
            if (typeOfArray is ArrayTypeNode)
                throw new TypeCheckException("Array don't have array type");
            var typeOfElementInArray = ((ArrayTypeNode)typeOfArray).ElementType;
            if (typeOfElementInArray == null)
                throw new TypeCheckException("Type of element in array is not set.");
            foreach (var expression in node.Elements)
            {
                if (expression.ExpressionType == null)
                    throw new TypeCheckException("Array contains value with not specified type.");
                if (!TypeNode.Compare(typeOfElementInArray,expression.ExpressionType))
                    throw new TypeCheckException("Inpropper Type of element in array.");
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
            if (Int32.TryParse(value, out out_int))
                _set_of_types.Add(NamedTypeNode.IntType());
            char out_char;
            if (Char.TryParse(value, out out_char))
                _set_of_types.Add(NamedTypeNode.CharType());
            return _set_of_types;
        }

        //try to find definition type on base of Value
        public override void Visit(AtomNode node)
        {
            base.Visit(node);
            if (node.Value == null)
                throw new TypeCheckException("Value is not set.");
            HashSet<TypeNode> deducedType = deduceType(node.Value);
            if (!deducedType.Contains(node.VariableType))
                throw new TypeCheckException("Type of Const is not consistent with deducated Type.");
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
            if (!TypeNode.Compare(NamedTypeNode.IntType(),node.Index.ExpressionType))
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
                throw new TypeCheckException("Inproper number of arguments.");
            
            for (int i = 0; i < listOfFunctionArguments.Count; i++)
            {
                if (!TypeNode.Compare(listOfFunctionArguments[i].Type,listOfFunctionCallArguments[i].ExpressionType))
                    throw new Exception(String.Format("Argument {0} has inproper type.", i));
            }

            node.ExpressionType = node.Definition.ResultType;
        }

        //check if body returns the same as set type
        public override void Visit(FunctionDefinitionNode node)
        {
            base.Visit(node);
            if (TypeNode.Compare(node.ResultType, node.Body.ExpressionType))
                throw new TypeCheckException("Body don't return the same type as set type.");
            node.ExpressionType = NamedTypeNode.VoidType();
        }

        //check if condition is bool and set common return type of Then and Else
        public override void Visit(IfNode node)
        {
            base.Visit(node);
            if(!TypeNode.Compare(NamedTypeNode.BoolType(), node.Condition.ExpressionType))
                throw new Exception("Inpropper type in if condition");
            if(!node.HasElse || !TypeNode.Compare(node.Then.ExpressionType, node.Else.ExpressionType))
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
                if (!TypeNode.Compare(while_elem.ExpressionType, node.Value.ExpressionType))
                    throw new TypeCheckException("Value Type is not correct for returned while type.");
            node.ExpressionType = node.Value.ExpressionType;
        }

        //rewrite Operator Type
        public override void Visit(OperatorNode node)
        {
            base.Visit(node);
            //node.ExpressionType = node.Operator;
        }

        //write type the same as in array node and check if Left and Right have int type
        public override void Visit(SliceNode node)
        {
            base.Visit(node);
            if (!TypeNode.Compare(NamedTypeNode.IntType(), node.Left.ExpressionType))
                throw new TypeCheckException("Left is not int value.");
            if (!TypeNode.Compare(NamedTypeNode.IntType(), node.Right.ExpressionType))
                throw new TypeCheckException("Right is not int value.");
            node.ExpressionType = node.Array.ExpressionType;
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
            if (!TypeNode.Compare(node.Value.ExpressionType, node.Type))
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
            if(!TypeNode.Compare(NamedTypeNode.BoolType(), node.Condition.ExpressionType))
                throw new Exception("Inpropper type in if condition");
            if(!node.HasElse || !TypeNode.Compare(node.Body.ExpressionType, node.Else.ExpressionType))
                _type_to_set = NamedTypeNode.VoidType();
            else
                _type_to_set = node.Body.ExpressionType;
            if (node.ExpressionType == null)
                node.ExpressionType = _type_to_set;
            else
                if (!TypeNode.Compare(node.ExpressionType, _type_to_set))
                    throw new Exception("Type of Body and Else is not correct with set type.");
                else
                    node.ExpressionType = _type_to_set;
        }
    }
}
