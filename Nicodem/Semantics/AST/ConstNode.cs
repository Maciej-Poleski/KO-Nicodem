using Nicodem.Semantics.Visitors;

namespace Nicodem.Semantics.AST
{
	class ConstNode : ExpressionNode
	{
		public TypeNode VariableType { get; private set; }

		// TODO value field
		public object Value { get; set; }

		/// <summary>
		/// The only way to construct this node.
		/// Type cannot be unspecified.
		/// </summary>
		/// <param name="type">Type.</param>
		public ConstNode( TypeNode type )
		{
			this.VariableType = type;
		}

		public override void Accept (AbstractVisitor visitor)
		{
			visitor.Visit (this);
		}
	}
}

