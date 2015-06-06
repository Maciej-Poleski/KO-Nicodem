using Nicodem.Semantics.Visitors;
using System.Collections.Generic;
using Nicodem.Parser;
using Nicodem.Semantics.ExpressionGraph;
using System.Linq;

namespace Nicodem.Semantics.AST
{
	class FunctionCallNode : ExpressionNode
	{
        public string Name { get; set; }  // set during AST construction
        public IReadOnlyList<ExpressionNode> Arguments { get; set; }  // set during AST construction
        public FunctionDefinitionNode Definition { get; set; }

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

        public override T Accept<T>(ReturnedAbstractVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        protected override bool Compare(object rhs_)
        {
            var rhs = (FunctionCallNode)rhs_;
            return base.Compare(rhs) &&
                object.Equals(Name, rhs.Name) &&
                SequenceEqual(Arguments, rhs.Arguments) &&
                object.Equals(Definition, rhs.Definition);
        }
	}
}

