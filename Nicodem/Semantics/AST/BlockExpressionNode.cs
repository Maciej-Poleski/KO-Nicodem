using System.Collections.Generic;
using System.Diagnostics;
using Nicodem.Semantics.Visitors;
using Nicodem.Parser;

namespace Nicodem.Semantics.AST
{
	class BlockExpressionNode : ExpressionNode
	{
        public IEnumerable<ExpressionNode> Elements { get; set; } // set during AST construction
        
        #region implemented abstract members of Node

        public override void BuildNode<TSymbol>(IParseTree<TSymbol> parseTree)
        {
            // Block -> "{" Expression* "}"
            var childs = ASTBuilder.ChildrenEnumerator(parseTree);
            childs.MoveNext(); // go to "{"
            Debug.Assert(childs.MoveNext()); // enter the brackets
            var elems = new List<ExpressionNode>();
            while (!ASTBuilder.EatSymbol("}", childs)) // collect expressions until closing "}"
            {
                elems.Add(ExpressionNode.GetExpressionNode(childs.Current));
            }
            Elements = elems;
        }

        #endregion

		public override void Accept (AbstractVisitor visitor)
		{
			visitor.Visit (this);
		}
	}
}

