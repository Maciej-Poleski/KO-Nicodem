using Nicodem.Semantics.Visitors;
using Nicodem.Parser;
using Nicodem.Backend;

namespace Nicodem.Semantics.AST
{
	class VariableDefNode : VariableDeclNode
	{
		public ExpressionNode Value { get; set; }
		public Location VariableLocation { get; set; }
        
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

