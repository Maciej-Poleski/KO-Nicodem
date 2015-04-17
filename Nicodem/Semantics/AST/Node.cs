using Nicodem.Source;
using Nicodem.Parser;
using Nicodem.Semantics.Visitors;

namespace Nicodem.Semantics.AST
{
	abstract class Node
	{
		public IFragment Fragment { get; set; }

        public abstract void BuildNode<TSymbol>(IParseTree<TSymbol> parseTree) where TSymbol:ISymbol<TSymbol>;

		public virtual void Accept (AbstractVisitor visitor)
		{
			visitor.Visit (this);
		}
	}
}

