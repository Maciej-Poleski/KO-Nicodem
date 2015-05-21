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
        private List<Vertex> nodes_to_next_jmp = new List<Vertex>();
        private Dictionary<ExpressionNode, ExpressionNode> changed_nodes = new Dictionary<ExpressionNode, ExpressionNode>();
        private int temporary_counter = 0;
        private List<Vertex> graph_vertex = new List<Vertex>();
        private List<WhileNode> while_stack = new List<WhileNode>();
        private Dictionary<LoopControlMode, Dictionary<WhileNode, List<Vertex>>> while_for_loop_control = new Dictionary<LoopControlMode, Dictionary<WhileNode, List<Vertex>>>();

        public List<Vertex> Graph
        {
            get
            {
                return graph_vertex;
            }
        }

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

        public override void Visit(ArrayNode node)
        {
            base.Visit(node);
            var new_elements = new List<ExpressionNode>();
            foreach (var element in node.Elements)
            {
                if (changed_nodes.ContainsKey(element))
                    new_elements.Add(changed_nodes[element]);
                else
                    new_elements.Add(element);
            }
            node.Elements = new_elements;
        }

        public override void Visit(BlockExpressionNode node)
        {
            base.Visit(node);
            foreach (var expression in node.Elements)
            {
                OneJumpVertex next_vertex = new OneJumpVertex(null, expression);
                graph_vertex.Add(next_vertex);
                AddNextJump(next_vertex);
                nodes_to_next_jmp.Add(next_vertex);
            }
        }

        public override void Visit(IfNode node)
        {
            base.Visit(node);

            VariableDeclNode t = new VariableDeclNode();
            t.Name = "T" + temporary_counter;
            VariableUseNode t_use = new VariableUseNode();
            t_use.Declaration = t;
            t_use.Name = "T" + temporary_counter;

            OperatorNode assign_then = new OperatorNode();
            assign_then.Operator = OperatorType.ASSIGN;
            assign_then.Arguments = new List<ExpressionNode> { t_use, node.Then };
            OneJumpVertex then_vertex = new OneJumpVertex(null, assign_then);
            graph_vertex.Add(then_vertex);

            ConditionalJumpVertex condition_vertex = new ConditionalJumpVertex(then_vertex, null, node.Condition);
            graph_vertex.Add(condition_vertex);
            AddNextJump(condition_vertex);

            nodes_to_next_jmp.Add(then_vertex);

            if (node.HasElse)
            {
                OperatorNode assign_else = new OperatorNode();
                assign_else.Operator = OperatorType.ASSIGN;
                assign_else.Arguments = new List<ExpressionNode> { t_use, node.Else };
                OneJumpVertex else_vertex = new OneJumpVertex(null, assign_else);
                graph_vertex.Add(else_vertex);
                nodes_to_next_jmp.Add(else_vertex);
                condition_vertex.FalseJump = else_vertex;
            }
            else{
                nodes_to_next_jmp.Add(condition_vertex);
            }

            changed_nodes[node] = t_use;
            temporary_counter++;
        }

        public override void Visit(LoopControlNode node)
        {
            base.Visit(node);

            if (node.Depth > while_stack.Count) throw new Exception("Not enough whiles on stack.");
            WhileNode loop_control_while = while_stack[while_stack.Count - node.Depth - 1];

            OneJumpVertex loop_control_vertex = new OneJumpVertex(null, node.Value);

            if (while_for_loop_control.ContainsKey(node.Mode) && while_for_loop_control[node.Mode].ContainsKey(loop_control_while))
                while_for_loop_control[node.Mode][loop_control_while].Add(loop_control_vertex);
            else
                while_for_loop_control[node.Mode][loop_control_while] = new List<Vertex>{loop_control_vertex};
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
            while_stack.Add(node);
            base.Visit(node);

            ConditionalJumpVertex condition_vertex = new ConditionalJumpVertex(null, null, node.Condition);
            graph_vertex.Add(condition_vertex);
            AddNextJump(condition_vertex);

            OneJumpVertex body_vertex = new OneJumpVertex(condition_vertex, node.Body);
            graph_vertex.Add(body_vertex);

            condition_vertex.TrueJump = body_vertex;
            nodes_to_next_jmp.Add(condition_vertex);

            if (while_for_loop_control.ContainsKey(LoopControlMode.LCM_BREAK) && while_for_loop_control[LoopControlMode.LCM_BREAK].ContainsKey(node))
                foreach (var loop_control_vetex in while_for_loop_control[LoopControlMode.LCM_BREAK][node])
                    nodes_to_next_jmp.Add(loop_control_vetex);
            if (while_for_loop_control.ContainsKey(LoopControlMode.LCM_CONTINUE) && while_for_loop_control[LoopControlMode.LCM_CONTINUE].ContainsKey(node))
                foreach (var loop_control_vetex in while_for_loop_control[LoopControlMode.LCM_CONTINUE][node])
                    ((OneJumpVertex)loop_control_vetex).Jump = body_vertex;
            while_stack.Remove(node);
        }

    }
}
