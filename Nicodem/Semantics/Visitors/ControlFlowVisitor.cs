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
        Dictionary<ExpressionNode, ExpressionNode> changed_nodes = new Dictionary<ExpressionNode, ExpressionNode>();
        private int temporary_counter = 0;
        public List<Vertex> Graph { public get; private set; }

        private void AddNextJump(Vertex next_vertex)
        {
            foreach (var list_vertex in nodes_to_next_jmp)
            {
                if (list_vertex is OneJumpVertex)
                    ((OneJumpVertex)list_vertex).Jump = next_vertex;
                if (list_vertex is ConditionalJumpVertex)
                    ((ConditionalJumpVertex)list_vertex).FalseJump = next_vertex;
            }

            nodes_to_next_jmp = new List<Vertex>();
        }

        public override void Visit(BlockExpressionNode node)
        {
            base.Visit(node);
            foreach (var expression in node.Elements)
            {
                var next_vertex = new OneJumpVertex(null, expression);
                AddNextJump(next_vertex);
                nodes_to_next_jmp.Add(next_vertex);
            }
        }

        public override void Visit(IfNode node)
        {
            base.Visit(node);
            //vertex then
            VariableDefNode t_then = new VariableDefNode();
            t_then.Name = "T" + temporary_counter;
            VariableUseNode t_then_use = new VariableUseNode();
            t_then_use.Declaration = t_then;
            t_then_use.Name = "T" + temporary_counter;
            var then_vertex = new OneJumpVertex(null, node.Then); //TODO T1 = then.Expression
            ConditionalJumpVertex condition_vertex = new ConditionalJumpVertex(then_vertex, null, node.Condition);
            //jump to condtion
            AddNextJump(condition_vertex);
            //queue true path
            nodes_to_next_jmp.Add(then_vertex);
            //vertex else
            if (node.HasElse)
            {
                var else_vertex = new OneJumpVertex(null, node.Else); //TODO T1 = else.Expression
                nodes_to_next_jmp.Add(else_vertex);
                condition_vertex.FalseJump = else_vertex;
            }
            else{
                nodes_to_next_jmp.Add(condition_vertex);
            }
            //vertex z T1
            changed_nodes[node] = null; //TODO zmiana na T1
            temporary_counter++;
        }

        public override void Visit(LoopControlNode node)
        {
            base.Visit(node);
        }

        public override void Visit(OperatorNode node)
        {
            base.Visit(node);
            var new_arguments = new List<ExpressionNode>();
            foreach(var argument in node.Arguments)
            {
                if (changed_nodes.ContainsKey(argument))
                    new_arguments.Add(changed_nodes[argument]);
                else
                    new_arguments.Add(argument);
            }
            node.Arguments = new_arguments;
        }

        public override void Visit(WhileNode node)
        {
            base.Visit(node);
            var condition_vertex = new ConditionalJumpVertex(null, null, node.Condition);
            AddNextJump(condition_vertex);
            var body_vertex = new OneJumpVertex(condition_vertex, node.Body);
            condition_vertex.TrueJump = body_vertex;
            nodes_to_next_jmp.Add(condition_vertex);
        }

    }
}
