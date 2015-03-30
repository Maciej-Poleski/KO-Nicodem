namespace Nicodem.Semantics.AST
{
	class WhileNode : ExpressionNode
	{
		public ExpressionNode Condition { get; set; }
		public ExpressionNode Body { get; set; }
		public ExpressionNode Else { get; set; }
	}
}

