namespace Nicodem.Semantics.AST
{
	class VariableDefNode : ExpressionNode
	{
		public string Name { get; set; }
		public TypeNode VariableType { get; set; }
		public ExpressionNode Value { get; set; }
	}
}

