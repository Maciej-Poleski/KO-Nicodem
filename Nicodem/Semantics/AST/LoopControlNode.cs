using Nicodem.Semantics.Visitors;
using Nicodem.Parser;

namespace Nicodem.Semantics.AST
{
	enum LoopControlMode
	{
		LCM_BREAK,
		LCM_CONTINUE
	}

	class LoopControlNode : ExpressionNode
	{
		public LoopControlMode Mode { get; set; }
		public int Depth { get; set; }
		public ExpressionNode Value { get; set; }
        
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

