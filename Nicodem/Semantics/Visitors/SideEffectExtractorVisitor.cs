using Nicodem.Semantics.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nicodem.Semantics.Visitors
{
    class SideEffectExtractorVisitor : AbstractRecursiveVisitor
    {
        private List<ExpressionNode> trees = new List<ExpressionNode>();

        public IEnumerable<ExpressionNode> Trees { get { return trees; } }

        // HasSideEffects must remain defined for all ExpressionNode types.
        private bool HasSideEffects(ConstNode node) { return false; }
        private bool HasSideEffects(IfNode node) { return false; }
        private bool HasSideEffects(OperationNode node) { throw new NotImplementedException(); }
        private bool HasSideEffects(OperatorNode node) { throw new NotImplementedException(); }
        private bool HasSideEffects(ElementNode node) { return false; }
        private bool HasSideEffects(VariableDeclNode node) { return false; }
        private bool HasSideEffects(VariableDefNode node) { return true; }
        private bool HasSideEffects(LoopControlNode node) { return false; }
        private bool HasSideEffects(FunctionCallNode node) { return true; }
        private bool HasSideEffects(VariableUseNode node) { return false; }
        private bool HasSideEffects(BlockExpressionNode node) { return false; }
        // TODO(guspiel): I'm not sure about FunctionDefinitionExpression.
        private bool HasSideEffects(FunctionDefinitionExpression node) { return true; } 
        private bool HasSideEffects(SliceNode node) { return false; }
        private bool HasSideEffects(WhileNode node) { return false; }
        // HasSideEffects should NOT be implemented for ExpressionNode.
        private bool HasSideEffects(ExpressionNode node) { throw new NotImplementedException(); } 

        override public void Visit(OperatorNode node)
        {
            base.Visit(node);

            switch (node.Operator) {
                case OperatorType.OT_PLUS:
                // case OperatorType.OT_MINUS:
                // ... 
                    var newArgs = new List<ExpressionNode>();
                    foreach (var arg in node.Arguments) {
                        if (HasSideEffects(arg)) {
                            var decl = new VariableDeclNode();
                            var def = new VariableDefNode();
                            var use = new VariableUseNode();
                            trees.Add(def);
                            newArgs.Add(use);
                        } else {
                            newArgs.Add(arg);
                        }
                    }
                    break;
                case OperatorType.OT_ASSIGNMENT:
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
