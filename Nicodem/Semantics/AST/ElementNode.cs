namespace Nicodem.Semantics.AST
{
	class ElementNode : ExpressionNode
	{
		public ExpressionNode Array { get; set; }
		public ExpressionNode Index { get; set; }
	}
}

