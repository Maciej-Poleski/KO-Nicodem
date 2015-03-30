namespace Nicodem.Semantics.AST
{
	class VariableUseNode : ExpressionNode
	{
		public string Name { get; set; }
		public VariableDefNode Definition { get; set; }
	}
}

