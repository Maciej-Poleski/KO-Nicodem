namespace Nicodem.Semantics.AST
{
	class FunctionCallNode : ExpressionNode
	{
		public string Name { get; set; }
		public ExpressionNode Arguments { get; set; }
	}
}

