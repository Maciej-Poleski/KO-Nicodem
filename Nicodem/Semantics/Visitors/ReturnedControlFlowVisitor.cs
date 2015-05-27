using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nicodem.Semantics.ExpressionGraph;
using Nicodem.Semantics.AST;
using System.Diagnostics;

namespace Nicodem.Semantics.Visitors
{
    class ReturnedControlFlowVisitor : ReturnedAbstractVisitor<SubExpressionGraph>
    {
        private int temporary_counter = 0;
        private List<WhileNode> while_stack = new List<WhileNode>();
        private Dictionary<LoopControlMode, Dictionary<WhileNode, List<Vertex>>> while_for_loop_control = new Dictionary<LoopControlMode, Dictionary<WhileNode, List<Vertex>>>();
        /// <summary>
        /// Create concatiantion of thwo SubExpressionGraphs
        /// </summary>
        /// <param name="current">First SubExpressionGraph</param>
        /// <param name="next">Second SubExpressionGraph</param>
        /// <returns>SubExpressionGraph as concatination of current and next</returns>
        private SubExpressionGraph ConcatSubGraph(SubExpressionGraph current, SubExpressionGraph next)
        {
            if (current == null || next == null)
                throw new Exception("One Of Subgraph is null");

            Debug.Assert(current.Graph.Count == 0 && next.Graph.Count == 0);

            if (current.Start == null || current.End == null)
            {
                return new SubExpressionGraph(next.Start, next.Graph, next.End);
            }
            if (next.Start == null || next.End == null)
            {
                return new SubExpressionGraph(current.Start, current.Graph, current.End);
            }
            if(current.End is ConditionalJumpVertex)
                throw new Exception("End Conditional Vertex");
            ((OneJumpVertex)current.End).Jump = next.Start;
            
            return new SubExpressionGraph(current.Start, new List<Vertex>(current.Graph.Concat(next.Graph)), next.End);
        }
        /// <summary>
        /// Create simply SubExpressionGraph with one Vertex.
        /// </summary>
        /// <param name="node">ExpressionNode which should be in vertex</param>
        /// <returns>SubExpressionGraph</returns>
        private SubExpressionGraph CreateOneVertexSubGraph(ExpressionNode node)
        {
            var vertex = new OneJumpVertex(null, node);
            return new SubExpressionGraph(vertex, new List<Vertex>{vertex}, vertex);
        }
        /// <summary>
        /// Check if there is If node, and we should update Value of node by temporary variable
        /// </summary>
        /// <param name="child">ExpressionNode</param>
        /// <param name="child_graph">SubExpressionGraph</param>
        /// <returns>New Value of Expression from parameter.</returns>
        private ExpressionNode GetUpdatedExpressionNodeValue(ExpressionNode child, SubExpressionGraph child_graph)
        {
            if (child_graph != null && child_graph.If != null)
                return child_graph.If;
            return child;
        }

        /// <summary>
        /// Update list of arguments or elements.
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns>SubExpressionGraph extracted from elements and List of new node values.</returns>
        private KeyValuePair<SubExpressionGraph, List<ExpressionNode>> UpdateList(IEnumerable<ExpressionNode> nodes)
        {
            var sub_graph = new SubExpressionGraph();
            var new_elements = new List<ExpressionNode>();
            
            foreach (var child in nodes)
                if (!ReferenceEquals(child, null))
                {
                    var child_graph = child.Accept(this);
                    sub_graph = ConcatSubGraph(sub_graph, child_graph);
                    new_elements.Add(GetUpdatedExpressionNodeValue(child, child_graph));
                }
            return new KeyValuePair<SubExpressionGraph, List<ExpressionNode>>(sub_graph, new_elements);
        }

        //Update each of row in array and return subgraph with this node
        public override SubExpressionGraph Visit(ArrayNode node)
        {
            var sub_graph = new SubExpressionGraph();

            if (!ReferenceEquals(node.Elements, null))
            {
                var elements = UpdateList(node.Elements);
                sub_graph = elements.Key;
                node.Elements = elements.Value;
            }

            sub_graph = ConcatSubGraph(sub_graph, CreateOneVertexSubGraph(node));
               
            return sub_graph;
        }

        //??
        public override SubExpressionGraph Visit(AtomNode node)
        {
            return base.Visit(node);
        }

        //Replace IfNodes and link sub_graph of childs
        public override SubExpressionGraph Visit(BlockExpressionNode node)
        {
            var sub_graph = new SubExpressionGraph();
            var new_elements = new List<ExpressionNode>();

            if (!ReferenceEquals(node.Elements, null))
            {
                foreach (var child in node.Elements)
                    if (!ReferenceEquals(child, null))
                    {
                        var child_graph = child.Accept(this);
                        if (child_graph == null)
                            throw new Exception("SubGraph of child is null");
                        sub_graph = ConcatSubGraph(sub_graph, child_graph);
                        new_elements.Add(GetUpdatedExpressionNodeValue(child, child_graph));
                    }
                node.Elements = new_elements;
            }

            return sub_graph;
        }

        //attach array and index sub_graph
        public override SubExpressionGraph Visit(ElementNode node)
        {
            var sub_graph = new SubExpressionGraph();

            if (!ReferenceEquals(node.Index, null))
            {
                var index_graph = node.Index.Accept(this);
                sub_graph = ConcatSubGraph(sub_graph, index_graph);
                node.Index = GetUpdatedExpressionNodeValue(node.Index, index_graph);
            }

            if (!ReferenceEquals(node.Array, null))
            {
                var array_graph = node.Array.Accept(this);
                sub_graph = ConcatSubGraph(sub_graph, array_graph);
            }

            sub_graph = ConcatSubGraph(sub_graph, CreateOneVertexSubGraph(node));

            return sub_graph;
        }

        //czy odwiedza arguemnty i function definition?
        public override SubExpressionGraph Visit(FunctionCallNode node)
        {
            var sub_graph = new SubExpressionGraph();

            if (!ReferenceEquals(node.Arguments, null))
            {
                var arguments = UpdateList(node.Arguments);
                node.Arguments = arguments.Value;
                sub_graph = arguments.Key;
            }

            sub_graph = ConcatSubGraph(sub_graph, CreateOneVertexSubGraph(node));

            return sub_graph;
        }

        // No visit, return empty BlockExpressioNode
        public override SubExpressionGraph Visit(FunctionDefinitionNode node)
        {
            return CreateOneVertexSubGraph(new BlockExpressionNode());
        }

        //Create diamond graph with CondtionVertex, mark flag IF
        public override SubExpressionGraph Visit(IfNode node)
        {
            var sub_graph = new SubExpressionGraph();

            if (!ReferenceEquals(node.Condition, null))
            {
                var condition_graph = node.Condition.Accept(this);
                sub_graph = ConcatSubGraph(sub_graph, condition_graph);
                node.Condition = GetUpdatedExpressionNodeValue(node.Condition, condition_graph);
            }

            if (!ReferenceEquals(node.Then, null))
            {
                var then_graph = node.Then.Accept(this);
                sub_graph = ConcatSubGraph(sub_graph, then_graph);
                node.Then = GetUpdatedExpressionNodeValue(node.Then, then_graph);
            }

            if (node.HasElse)
            {
                var else_graph = node.Else.Accept(this);
                sub_graph = ConcatSubGraph(sub_graph, else_graph);
                node.Else = GetUpdatedExpressionNodeValue(node.Else, else_graph);
            }

            VariableDefNode t = new VariableDefNode();
            t.Name = "T" + temporary_counter;
            AtomNode _0 = new AtomNode(NamedTypeNode.IntType());
            _0.Value = "0";
            t.Value = _0;
            VariableUseNode t_use = new VariableUseNode();
            t_use.Declaration = t;
            t_use.Name = "T" + temporary_counter;

            OperatorNode assign_then = new OperatorNode();
            assign_then.Operator = OperatorType.ASSIGN;
            assign_then.Arguments = new List<ExpressionNode> { t_use, node.Then };
            
            OneJumpVertex then_vertex = new OneJumpVertex(null, assign_then);
            OneJumpVertex end_if = new OneJumpVertex(null, t_use);
            ConditionalJumpVertex condition_vertex = new ConditionalJumpVertex(then_vertex, end_if, node.Condition);

            var vertex_in_if = new List<Vertex>{condition_vertex, then_vertex};

            if (node.HasElse)
            {
                OperatorNode assign_else = new OperatorNode();
                assign_else.Operator = OperatorType.ASSIGN;
                assign_else.Arguments = new List<ExpressionNode> { t_use, node.Else };

                OneJumpVertex else_vertex = new OneJumpVertex(null, assign_else);

                condition_vertex.FalseJump = else_vertex;
                else_vertex.Jump = end_if;

                vertex_in_if.Add(else_vertex);
            }
            else {
                condition_vertex.FalseJump = end_if;
            }

            vertex_in_if.Add(end_if);

            var if_sub_graph = new SubExpressionGraph(condition_vertex, vertex_in_if, end_if);
            sub_graph = ConcatSubGraph(sub_graph, if_sub_graph);

            sub_graph.If = t_use;
            temporary_counter++;

            return sub_graph;
        }

        //Create LoopControl SubExpressionGraph and remember End's Vertex in this loop control, in while this End will be changed acording break or continue label
        public override SubExpressionGraph Visit(LoopControlNode node)
        {
            var sub_graph = new SubExpressionGraph();

            if (!ReferenceEquals(node.Value, null))
            {
                var value_graph = node.Value.Accept(this);
                sub_graph = ConcatSubGraph(sub_graph, value_graph);
                node.Value = GetUpdatedExpressionNodeValue(node.Value, value_graph);
            }

            if (node.Depth > while_stack.Count) throw new Exception("Not enough whiles on stack.");
            WhileNode loop_control_while = while_stack[while_stack.Count - node.Depth - 1];

            SubExpressionGraph loop_control_graph = CreateOneVertexSubGraph(node.Value);
            OneJumpVertex loop_control_vertex = (OneJumpVertex)loop_control_graph.Start;

            if (while_for_loop_control.ContainsKey(node.Mode) && while_for_loop_control[node.Mode].ContainsKey(loop_control_while))
                while_for_loop_control[node.Mode][loop_control_while].Add(loop_control_vertex);
            else
                while_for_loop_control[node.Mode][loop_control_while] = new List<Vertex> { loop_control_vertex };

            sub_graph = ConcatSubGraph(sub_graph, loop_control_graph);

            return sub_graph;
        }

        //Update list of arguments return subgraph with OperatorNode
        public override SubExpressionGraph Visit(OperatorNode node)
        {
            var sub_graph = new SubExpressionGraph();

            if (!ReferenceEquals(node.Arguments, null))
            {
                var arguments = UpdateList(node.Arguments);
                node.Arguments = arguments.Value;
                sub_graph = arguments.Key;
            }

            sub_graph = ConcatSubGraph(sub_graph, CreateOneVertexSubGraph(node));

            return sub_graph;
        }

        //Update and return subgraph with SliceNode
        public override SubExpressionGraph Visit(SliceNode node)
        {
            var sub_graph = new SubExpressionGraph();

            if (node.HasLeft)
            {
                var left_graph = node.Left.Accept(this);
                sub_graph = ConcatSubGraph(sub_graph, left_graph);
                node.Left = GetUpdatedExpressionNodeValue(node.Left, left_graph);
            }

            if (node.HasRight)
            {
                var right_graph = node.Right.Accept(this);
                sub_graph = ConcatSubGraph(sub_graph, right_graph);
                node.Right = GetUpdatedExpressionNodeValue(node.Right, right_graph);
            }

            if (!ReferenceEquals(node.Array, null))
            {
                var array_graph = node.Array.Accept(this);
                sub_graph = ConcatSubGraph(sub_graph, array_graph);//?
            }

            sub_graph = ConcatSubGraph(sub_graph, CreateOneVertexSubGraph(node));

            return sub_graph;
        }

        //Update and return subgraph with VariableDefNode
        public override SubExpressionGraph Visit(VariableDefNode node)
        {
            var sub_graph = new SubExpressionGraph();

            if (!ReferenceEquals(node.Value, null))
            {
                var value_graph = node.Value.Accept(this);
                sub_graph = ConcatSubGraph(sub_graph, value_graph);
                node.Value = GetUpdatedExpressionNodeValue(node.Value, value_graph);
            }

            sub_graph = ConcatSubGraph(sub_graph, CreateOneVertexSubGraph(node));

            return sub_graph;
        }


        public override SubExpressionGraph Visit(WhileNode node)
        {
            while_stack.Add(node);

            var sub_graph = new SubExpressionGraph();

            var begin_while = CreateOneVertexSubGraph(new BlockExpressionNode());

            if (!ReferenceEquals(node.Condition, null))
            {
                var condition_graph = node.Condition.Accept(this);
                sub_graph = ConcatSubGraph(sub_graph, condition_graph);
                node.Condition = GetUpdatedExpressionNodeValue(node.Condition, condition_graph);
            }

            if (!ReferenceEquals(node.Body, null))
            {
                var body_graph = node.Body.Accept(this);
                sub_graph = ConcatSubGraph(sub_graph, body_graph);
                node.Body = GetUpdatedExpressionNodeValue(node.Body, body_graph);
            }

            if (node.HasElse)
            {
                var else_graph = node.Else.Accept(this);
                sub_graph = ConcatSubGraph(sub_graph, else_graph);
                node.Else = GetUpdatedExpressionNodeValue(node.Else, else_graph);
            }

            List<Vertex> while_graph_vertex = new List<Vertex>();

            ConditionalJumpVertex condition_vertex = new ConditionalJumpVertex(null, null, node.Condition);
            while_graph_vertex.Add(condition_vertex);

            OneJumpVertex body_vertex = new OneJumpVertex(condition_vertex, node.Body);
            while_graph_vertex.Add(body_vertex);

            OneJumpVertex end_while_vertex = new OneJumpVertex(null, new BlockExpressionNode());

            condition_vertex.TrueJump = body_vertex;
            body_vertex.Jump = begin_while.Start;

            if (node.HasElse)
            {
                OneJumpVertex else_vertex = new OneJumpVertex(end_while_vertex, node.Else);
                while_graph_vertex.Add(else_vertex);
                condition_vertex.FalseJump = else_vertex;
            }
            else
            {
                condition_vertex.FalseJump = end_while_vertex;
            }

            while_graph_vertex.Add(end_while_vertex);

            sub_graph = ConcatSubGraph(sub_graph, new SubExpressionGraph(condition_vertex, while_graph_vertex, end_while_vertex));

            if (while_for_loop_control.ContainsKey(LoopControlMode.LCM_BREAK) && while_for_loop_control[LoopControlMode.LCM_BREAK].ContainsKey(node))
                foreach (var loop_control_vertex in while_for_loop_control[LoopControlMode.LCM_BREAK][node])
                    ((OneJumpVertex)loop_control_vertex).Jump = sub_graph.End;
            if (while_for_loop_control.ContainsKey(LoopControlMode.LCM_CONTINUE) && while_for_loop_control[LoopControlMode.LCM_CONTINUE].ContainsKey(node))
                foreach (var loop_control_vertex in while_for_loop_control[LoopControlMode.LCM_CONTINUE][node])
                    ((OneJumpVertex)loop_control_vertex).Jump = sub_graph.Start;

            while_stack.Remove(node);

            return sub_graph;
        }
    }
}
