using Nicodem.Source;
using Nicodem.Parser;
using Nicodem.Semantics.Visitors;
using Nicodem.Semantics.ExpressionGraph;
using System;

namespace Nicodem.Semantics.AST
{
	abstract class Node
	{
        public abstract void BuildNode<TSymbol>(IParseTree<TSymbol> parseTree) where TSymbol:ISymbol<TSymbol>;

		public virtual void Accept (AbstractVisitor visitor)
		{
			visitor.Visit (this);
		}

        public virtual T Accept<T>(ReturnedAbstractVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
        
        /* Not implemented yet.
        // Ignores Fragments.
        public override bool Equals(object obj)
        {
            return Compare(obj);
        }

        protected bool Compare(Node rhs)
        {
            return true;
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
         * */
	}
}

