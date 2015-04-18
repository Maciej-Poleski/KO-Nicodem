using System;
using Nicodem.Semantics.AST;

namespace Nicodem.Semantics.Visitors
{
	class NameResolutionVisitor : AbstractRecursiveVisitor
	{
        private SymbolTable<VariableDefNode> variableSymbolTable;
        private SymbolTable<FunctionNode> functionSymbolTable;

		public NameResolutionVisitor ()
		{
            variableSymbolTable = new SymbolTable<VariableDefNode> ();
            functionSymbolTable = new SymbolTable<FunctionNode>();
		}

		public override void Visit (BlockExpressionNode node)
		{
            variableSymbolTable.OpenBlock ();
            functionSymbolTable.OpenBlock ();
			base.Visit (node);
            variableSymbolTable.CloseBlock ();
            functionSymbolTable.CloseBlock ();
		}

		public override void Visit (VariableDefNode node)
		{
            variableSymbolTable.Insert (node.Name, node);
			base.Visit (node);
		}

		public override void Visit (VariableUseNode node)
		{
            node.Definition = variableSymbolTable.LookUp (node.Name);
			base.Visit (node);
		}

        public override void Visit (FunctionNode node)
        {
            functionSymbolTable.Insert (node.Name, node);
            base.Visit (node);
        }

        public override void Visit (FunctionCallNode node)
        {
            node.Definition = functionSymbolTable.LookUp (node.Name);
            base.Visit (node);
        }
	}
}

