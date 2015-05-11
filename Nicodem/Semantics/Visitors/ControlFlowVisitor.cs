using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nicodem.Semantics.AST;
using Nicodem.Semantics.ExpressionGraph;

namespace Nicodem.Semantics.Visitors
{
    class ControlFlowVisitor : AbstractRecursiveVisitor
    {
        List<Vertex> nodes_to_next_jmp = new List<Vertex>();
        Dictionary<ExpressionNode, ExpressionNode> dictionary_of_copied_nodes = new Dictionary<ExpressionNode, ExpressionNode>();

        public override void Visit(ProgramNode node)
        {
            base.Visit(node);
        }

        public override void Visit(ExpressionNode node)
        {
            base.Visit(node);
        }

        public override void Visit(ArrayTypeNode node)
        {
            base.Visit(node);
        }

        public override void Visit(FunctionDefinitionNode node)
        {
            base.Visit(node);
        }

        public override void Visit(ArrayNode node)
        {
            base.Visit(node);
        }

        public override void Visit(AtomNode node)
        {
            base.Visit(node);
        }

        public override void Visit(BlockExpressionNode node)
        {
            base.Visit(node);
        }

        public override void Visit(ElementNode node)
        {
            base.Visit(node);
        }

        public override void Visit(FunctionCallNode node)
        {
            base.Visit(node);
        }

        public override void Visit(IfNode node)
        {
            base.Visit(node);
            //vertex then
            var then_vertex = new OneJumpVertex(null, dictionary_of_copied_nodes[node.Then]); //T1 = then.Expression
            nodes_to_next_jmp.Add(then_vertex);
            //vertex else
            if (node.HasElse)
            {
                var else_vertex = new OneJumpVertex(null, dictionary_of_copied_nodes[node.Else]); //T1 = else.Expression
                nodes_to_next_jmp.Add(else_vertex);
                var condition_vertex = new ConditionalJumpVertex(then_vertex, else_vertex, node.Condition);
            }
            else{
                var condition_vertex = new ConditionalJumpVertex(then_vertex, null, node.Condition);
                nodes_to_next_jmp.Add(condition_vertex);
            }
            //dodaj do drzewa
            //vertex z T1
        }

        public override void Visit(LoopControlNode node)
        {
            base.Visit(node);
        }

        public override void Visit(SliceNode node)
        {
            base.Visit(node);
        }

        public override void Visit(VariableDeclNode node)
        {
            base.Visit(node);
        }

        public override void Visit(VariableDefNode node)
        {
            base.Visit(node);
        }

        public override void Visit(WhileNode node)
        {
            base.Visit(node);
        }

    }
}
