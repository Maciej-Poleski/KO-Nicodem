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
	}
}

