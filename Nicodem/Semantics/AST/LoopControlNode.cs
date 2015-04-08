using Nicodem.Semantics.Visitors;

namespace Nicodem.Semantics.AST
{
	enum LoopControlMode
	{
		LCM_BREAK,
		LCM_CONTINUE
	}

	class LoopControlNode : ExpressionNode
	{
		public LoopControlMode Mode { get; set; }
		public int Depth { get; set; }
		public ExpressionNode Value { get; set; }

		public override TResult Accept<TResult> (AbstractVisitor<TResult> visitor)
		{
			return visitor.Visit (this);
		}
	}
}

