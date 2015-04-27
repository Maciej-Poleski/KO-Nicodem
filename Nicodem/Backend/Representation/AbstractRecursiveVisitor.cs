using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nicodem.Backend.Representation
{
    public abstract class AbstractRecursiveVisitor : AbstractVisitor
    {
        public override void Visit(Node node)
        {
        }

        #region Node subclasses

        public override void Visit(LocationNode node)
        {
            Visit(node as Node);
        }

        public override void Visit(ConditionalJumpNode node)
        {
            Visit(node as Node);
            node.Condition.Accept(this);
        }

        public override void Visit(SequenceNode node)
        {
            Visit(node as Node);
            foreach (var chld in node.Sequence)
            {
                chld.Accept(this);
            }
        }

        public override void Visit(AssignmentNode node)
        {
            Visit(node as Node);
            node.Source.Accept(this);
            node.Target.Accept(this);
        }

        public override void Visit<TFunction>(FunctionCallNode<TFunction> node)
        {
            Visit(node as Node);
            foreach (var argument in node.Arguments)
            {
                argument.Accept(this);
            }
        }

        public override void Visit<TConstant>(ConstantNode<TConstant> node)
        {
            Visit(node as Node);
        }

        public override void Visit<TOperator>(OperatorNode<TOperator> node)
        {
            Visit(node as Node);
        }

        #region OperatorNode subclasses

        public override void Visit(UnaryOperatorNode node)
        {
            Visit(node as OperatorNode<UnaryOperatorType>);
            node.Operand.Accept(this);
        }

        public override void Visit(BinaryOperatorNode node)
        {
            Visit(node as OperatorNode<BinaryOperatorType>);
            node.LeftOperand.Accept(this);
            node.RightOperand.Accept(this);
        }

        #endregion

        #region LocationNode subclasses

        public override void Visit(RegisterNode node)
        {
            Visit(node as LocationNode);
        }

        public override void Visit(MemoryNode node)
        {
            Visit(node as LocationNode);
        }

        #region RegisterNode subclasses

        public override void Visit(HardwareRegisterNode node)
        {
            Visit(node as RegisterNode);
        }

        public override void Visit(TemporaryNode node)
        {
            Visit(node as RegisterNode);
        }

        #endregion
        #endregion
        #endregion
    }
}
