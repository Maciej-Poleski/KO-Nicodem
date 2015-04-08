using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nicodem.Semantics.AST;

namespace Nicodem.Semantics.Visitors
{
    class TypeCheckingVisitor : AbstractRecursiveVisitor
    {
        public override void Visit(ProgramNode node)
        {
            //last function
        }

        public override void Visit(FunctionNode node)
        {
            //TYPE rewrite
        }

        public override void Visit(ParameterNode node)
        {
            //TYPE rewrite
        }

        public override void Visit(ExpressionNode node)
        {
            //TYPE rewrite
        }

        public override void Visit(ArrayTypeNode node)
        {
            //TYPE rewrite
        }

        public override void Visit(ArrayNode node)
        {
            //check every TYPE is the same?
            //write this type as a whole array TYPE
        }

        public override void Visit(BlockExpressionNode node)
        {
            //last one expresion type?
        }

        public override void Visit(ConstNode node)
        {
            //TYPE rewrite
        }

        public override void Visit(ElementNode node)
        {
            //??
        }

        public override void Visit(FunctionCallNode node)
        {
            //??
        }

        public override void Visit(IfNode node)
        {
            //condition true
            //if then and else and type are the same return type
            //if then and not else return type then
            //else return unrecogizable
        }

        public override void Visit(LoopControlNode node)
        {
            //TYPE VALUE TYPE?
        }

        public override void Visit(OperationNode node)
        {
            //TYPE rewrite
        }

        public override void Visit(SliceNode node)
        {
            //TYPE rewrite array
        }

        public override void Visit(VariableDefNode node)
        {
            //TYPE rewrite
        }

        public override void Visit(VariableUseNode node)
        {
            //TYPE rewrite
        }

        public override void Visit(WhileNode node)
        {
            //check condition bool?
            //if hasElse then type is else
            //else type is body
        }
    }
}
