using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nicodem.Parser;
using Nicodem.Semantics.Visitors;
using Nicodem.Semantics.ExpressionGraph;

namespace Nicodem.Semantics.AST
{
    // TODO: when to use this class?
    class AtomNode : ConstNode
    {

        #region implemented abstract members of Node

        public override void BuildNode<TSymbol>(IParseTree<TSymbol> parseTree)
        {
            throw new System.NotImplementedException();
        }

        #endregion

        public AtomNode(TypeNode type) : base(type) { }

        public override void Accept(AbstractVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override SubExpressionGraph Accept(ReturnedAbstractVisitor<SubExpressionGraph> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
