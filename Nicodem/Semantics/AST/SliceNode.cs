using Nicodem.Semantics.Visitors;

namespace Nicodem.Semantics.AST
{
	class SliceNode : ExpressionNode
	{
		public ExpressionNode Array { get; set; }
		public ExpressionNode Left { get; set; }
		public ExpressionNode Right { get; set; }

		public bool HasLeft { get { return !ReferenceEquals (Left, null); } }
		public bool HasRight { get { return !ReferenceEquals (Right, null); } }

		public override void Accept (AbstractVisitor visitor)
		{
			visitor.Visit (this);
		}
	}
}

