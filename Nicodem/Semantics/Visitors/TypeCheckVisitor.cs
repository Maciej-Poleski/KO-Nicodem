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

        //check if body returns the same as set type
        public override void Visit(FunctionNode node)
        {
            base.Visit(node);
            if (node.Type.Equals(node.Body.ExpressionType))
                throw new TypeCheckException("Body don't return the same type as set type.");
        }

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
            foreach (var expression in node.Elements)
            {
                if (expression.ExpressionType == null)
                    throw new TypeCheckException("Array contains value with not specified type.");
                if (!typeOfElementInArray.Equals(expression.ExpressionType))
                    throw new TypeCheckException("Inpropper Type of element in array.");
            }
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

        private TypeNode deduceType(object value)
        {
            throw new NotImplementedException();
        }

        //try to find definition type on base of Value
        public override void Visit(ConstNode node)
        {
            base.Visit(node);
            TypeNode deducedType = deduceType(node.Value);
            if (!deducedType.Equals(node.VariableType))
                throw new TypeCheckException("Type of Const is not consistent with deducated Type.");
            node.ExpressionType = node.VariableType;
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
                throw new TypeCheckException("Inproper number of arguments.");
            
            for (int i = 0; i < listOfFunctionArguments.Count; i++)
            {
                if (!listOfFunctionArguments[i].Equals(listOfFunctionCallArguments[i]))
                    throw new Exception(String.Format("Argument {0} has inproper type.", i));
            }

            node.ExpressionType = node.Definition.Type;
        }

        //check if condition is bool and set common return type of Then and Else
        public override void Visit(IfNode node)
        {
            base.Visit(node);
            if(NamedTypeNode.BoolType().Equals(node.Condition.ExpressionType))
                throw new Exception("Inpropper type in if condition");
            if(!node.HasElse || node.Then.ExpressionType.Equals(node.Else.ExpressionType))
                node.ExpressionType = node.Then.ExpressionType;
            else
                node.ExpressionType = NamedTypeNode.VoidType();
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
        }

        //every arguments has the same type and set type to the arguements type
        public override void Visit(OperationNode node)
        {
            base.Visit(node);
            TypeNode argument_type = null;
            foreach (var argument in node.Arguments)
            {
                if (argument_type != null && !argument_type.Equals(argument.ExpressionType))
                    throw new TypeCheckException("Types are not the same.");
                argument_type = argument.ExpressionType;
            }
            if (argument_type == null)
                throw new TypeCheckException("Un recognized type of argument.");
            node.ExpressionType = argument_type;
        }

        //write type the same as in array node and check if Left and Right have int type
        public override void Visit(SliceNode node)
        {
            base.Visit(node);
            if (!NamedTypeNode.IntType().Equals(node.Left))
                throw new TypeCheckException("Left is not int value.");
            if (!NamedTypeNode.IntType().Equals(node.Right))
                throw new TypeCheckException("Right is not int value.");
            node.ExpressionType = node.Array.ExpressionType;
        }

        //value has the same type as definition and it is rewrite to my type
        public override void Visit(VariableDefNode node)
        {
            base.Visit(node);
            if (!node.Value.ExpressionType.Equals(node.VariableType))
                throw new TypeCheckException("Value type not agree with VariableType");
            node.ExpressionType = node.VariableType;
        }

        //rewerite type
        public override void Visit(VariableUseNode node)
        {
            base.Visit(node);
            node.ExpressionType = node.Definition.ExpressionType;
        }

        //check condtion type and set returned type as in if
        public override void Visit(WhileNode node)
        {
            _stack_of_while_node.Add(node);
            base.Visit(node);
            _stack_of_while_node.Remove(node);
            if(NamedTypeNode.BoolType().Equals(node.Condition.ExpressionType))
                throw new Exception("Inpropper type in if condition");
            if(!node.HasElse || node.Body.ExpressionType.Equals(node.Else.ExpressionType))
                node.ExpressionType = node.Body.ExpressionType;
            else
                node.ExpressionType = NamedTypeNode.VoidType();
        }
    }
}
