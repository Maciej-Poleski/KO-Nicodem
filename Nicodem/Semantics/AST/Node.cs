using Nicodem.Source;
using Nicodem.Semantics.Visitors;

namespace Nicodem.Semantics.AST
{
	abstract class Node
	{
		public IFragment Fragment { get; set; }

		public virtual void Accept (AbstractVisitor visitor)
		{
			visitor.Visit (this);
		}
	}
}

