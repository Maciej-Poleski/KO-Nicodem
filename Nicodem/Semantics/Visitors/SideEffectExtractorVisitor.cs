using Nicodem.Backend;
using Nicodem.Semantics.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brep = Nicodem.Backend.Representation;

namespace Nicodem.Semantics.Visitors
{
    class SideEffectExtractorVisitor : AbstractRecursiveVisitor
    {
        private List<ExpressionNode> trees = new List<ExpressionNode>();

        public IEnumerable<ExpressionNode> Trees { get { return trees; } }

        // HasSideEffects must remain defined for all ExpressionNode types.
        private bool HasSideEffects(ArrayNode node) { return false; }
        private bool HasSideEffects(AtomNode node) { return false; }
        private bool HasSideEffects(ConstNode node) { return false; }
        private bool HasSideEffects(IfNode node) { return false; }
        private bool HasSideEffects(OperatorNode node)
        {
            switch (node.Operator) {
                case OperatorType.PLUS_ASSIGN:
                case OperatorType.MINUS_ASSIGN:
                case OperatorType.MUL_ASSIGN:
                case OperatorType.DIV_ASSIGN:
                case OperatorType.MOD_ASSIGN:
                case OperatorType.BIT_SHIFT_UP_ASSIGN:
                case OperatorType.BIT_SHIFT_DOWN_ASSIGN:
                case OperatorType.BIT_AND_ASSIGN:
                case OperatorType.BIT_XOR_ASSIGN:
                case OperatorType.BIT_OR_ASSIGN:
                case OperatorType.PRE_INCREMENT:
                case OperatorType.PRE_DECREMENT:
                case OperatorType.POST_INCREMENT:
                case OperatorType.POST_DECREMENT:
                    return true;

                case OperatorType.OR:
                case OperatorType.AND:
                case OperatorType.BIT_OR:
                case OperatorType.BIT_XOR:
                case OperatorType.BIT_AND:
                case OperatorType.EQUAL:
                case OperatorType.NOT_EQUAL:
                case OperatorType.LESS:
                case OperatorType.LESS_EQUAL:
                case OperatorType.GREATER:
                case OperatorType.GREATER_EQUAL:
                case OperatorType.BIT_SHIFT_UP:
                case OperatorType.BIT_SHIFT_DOWN:
                case OperatorType.PLUS:
                case OperatorType.MINUS:
                case OperatorType.MUL:
                case OperatorType.DIV:
                case OperatorType.MOD:
                case OperatorType.UNARY_PLUS:
                case OperatorType.UNARY_MINUS:
                case OperatorType.NOT:
                case OperatorType.BIT_NOT:
                    return false;

                default: 
                    throw new NotImplementedException();
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
        private bool HasSideEffects(FunctionDefinitionNode node) { return true; } 
        private bool HasSideEffects(SliceNode node) { return false; }
        private bool HasSideEffects(WhileNode node) { return false; }
        // HasSideEffects should NOT be implemented for ExpressionNode. 
        // This notifies the programmer to implement the code for new node types.
        // UWAGA! Wiadomo co tu jest zle. Jct prosze pisac do mnie (G. Guspiel) lub dzwonic 607083266.
        private bool HasSideEffects(ExpressionNode node) { throw new NotImplementedException(); } 

        override public void Visit(OperatorNode node)
        {
            base.Visit(node);

            switch (node.Operator) {
                case OperatorType.OR:
                case OperatorType.AND:
                case OperatorType.BIT_OR:
                case OperatorType.BIT_XOR:
                case OperatorType.BIT_AND:
                case OperatorType.EQUAL:
                case OperatorType.NOT_EQUAL:
                case OperatorType.LESS:
                case OperatorType.LESS_EQUAL:
                case OperatorType.GREATER:
                case OperatorType.GREATER_EQUAL:
                case OperatorType.BIT_SHIFT_UP:
                case OperatorType.BIT_SHIFT_DOWN:
                case OperatorType.PLUS:
                case OperatorType.MINUS:
                case OperatorType.MUL:
                case OperatorType.DIV:
                case OperatorType.MOD:
                case OperatorType.UNARY_PLUS:
                case OperatorType.UNARY_MINUS:
                case OperatorType.NOT:
                case OperatorType.BIT_NOT:
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

                case OperatorType.ASSIGN:
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
