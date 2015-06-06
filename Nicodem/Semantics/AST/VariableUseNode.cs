using Nicodem.Semantics.Visitors;
using Nicodem.Parser;
using Nicodem.Semantics.ExpressionGraph;

namespace Nicodem.Semantics.AST
{
	class VariableUseNode : ExpressionNode
	{
        public string Name { get; set; } // set during AST construction
		public VariableDeclNode Declaration { get; set; }
        
        #region implemented abstract members of Node

        public override void BuildNode<TSymbol>(IParseTree<TSymbol> parseTree)
        {
            Name = parseTree.Fragment.GetOriginText();
        }

        #endregion

		public override void Accept (AbstractVisitor visitor)
		{
			visitor.Visit (this);
		}

        public override T Accept<T>(ReturnedAbstractVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        protected override bool Compare(object rhs_)
        {
            var rhs = (VariableUseNode)rhs_;
            return base.Compare(rhs) &&
                object.Equals(Name, rhs.Name) &&
                object.Equals(Declaration, rhs.Declaration);
        }
	}
}
