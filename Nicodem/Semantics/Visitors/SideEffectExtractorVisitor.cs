using Nicodem.Backend;
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
        // At the time of writing this code, nobody knew what OperationNode represents.
        private bool HasSideEffects(OperationNode node) { throw new NotImplementedException(); }
        private bool HasSideEffects(OperatorNode node)
        {
            switch (node.Operator) {
                case OperatorType.OT_ASSIGNMENT: return true;
                case OperatorType.OT_PLUS: return false;
                default: throw new NotImplementedException();
            }
        }
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
        // This notifies the programmer to implement the code for new node types.
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
                            var tempDef = new VariableDefNode();
                            tempDef.Name = "temp";
                            tempDef.Type = arg.ExpressionType;
                            tempDef.NestedUse = false;
                            tempDef.Value = arg;
                            tempDef.VariableLocation = new Temporary();
                            trees.Add(tempDef);
                            var tempUse = new VariableUseNode();
                            tempUse.Name = "temp";
                            tempUse.Declaration = tempDef;
                            newArgs.Add(tempUse);
                        } else {
                            newArgs.Add(arg);
                        }
                    }
                    node.Arguments = newArgs;
                    break;

                case OperatorType.OT_ASSIGNMENT:
                    // Do nothing. If node was a child, it would be extracted
                    // and this code would not be running. Therefore,
                    // node is a root. We accept side effects in roots.
                    break;

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
