namespace Nicodem.Semantics.AST
{
	class IfNode : ExpressionNode
	{
		public ExpressionNode Condition { get; set; }
		public ExpressionNode Then { get; set; }
		public ExpressionNode Else { get; set; }

		public bool HasElse { get { return !ReferenceEquals (Else, null); } }
	}
}

