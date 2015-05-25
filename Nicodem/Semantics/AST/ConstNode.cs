using Nicodem.Semantics.Visitors;
using Nicodem.Parser;
using Nicodem.Semantics.ExpressionGraph;

namespace Nicodem.Semantics.AST
{
    // TODO - is this class needed?
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

        public override SubExpressionGraph Accept(ReturnedAbstractVisitor<SubExpressionGraph> visitor)
        {
            return visitor.Visit(this);
        }
	}
}

