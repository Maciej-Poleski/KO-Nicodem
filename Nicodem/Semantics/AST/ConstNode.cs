using Nicodem.Semantics.Visitors;
using Nicodem.Parser;

namespace Nicodem.Semantics.AST
{
	abstract class ConstNode : ExpressionNode
	{
		public TypeNode VariableType { get; private set; }

		// TODO value field
		public string Value { get; set; }

		/// <summary>
		/// The only way to construct this node.
		/// Type cannot be unspecified.
		/// </summary>
		/// <param name="type">Type.</param>
		public ConstNode( TypeNode type )
		{
			this.VariableType = type;
		}
        
        #region implemented abstract members of Node

        public override void BuildNode<TSymbol>(IParseTree<TSymbol> parseTree)
        {
            throw new System.NotImplementedException();
        }

        #endregion

		public override void Accept (AbstractVisitor visitor)
		{
			visitor.Visit (this);
		}
	}
}

