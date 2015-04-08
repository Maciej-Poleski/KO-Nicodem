using Nicodem.Source;
using Nicodem.Semantics.Visitors;

namespace Nicodem.Semantics.AST
{
	abstract class Node
	{
		public IFragment Fragment { get; set; }

		public virtual TResult Accept<TResult> (AbstractVisitor<TResult> visitor)
		{
			return visitor.Visit (this);
		}
	}
}

