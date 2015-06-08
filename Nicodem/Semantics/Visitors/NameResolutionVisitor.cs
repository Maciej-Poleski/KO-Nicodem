using System;
using Nicodem.Semantics.AST;

namespace Nicodem.Semantics.Visitors
{
	class NameResolutionVisitor : AbstractRecursiveVisitor
	{
        private SymbolTable<VariableDeclNode> variableSymbolTable;
        private SymbolTable<FunctionDefinitionNode> functionSymbolTable;

		public NameResolutionVisitor ()
		{
            variableSymbolTable = new SymbolTable<VariableDeclNode> ();
            functionSymbolTable = new SymbolTable<FunctionDefinitionNode>();
		}

		public override void Visit (BlockExpressionNode node)
		{
            variableSymbolTable.OpenBlock ();
            functionSymbolTable.OpenBlock ();
			base.Visit (node);
            variableSymbolTable.CloseBlock ();
            functionSymbolTable.CloseBlock ();
		}

		public override void Visit (VariableDeclNode node)
		{
            variableSymbolTable.Insert (node.Name, node);
			Console.WriteLine ("DeclNode: " + node.Name);
			base.Visit (node);
		}

		public override void Visit (VariableUseNode node)
		{
			Console.WriteLine ("UseNode: " + node.Name);
            node.Declaration = variableSymbolTable.LookUp (node.Name);
			base.Visit (node);
		}

        public override void Visit (FunctionDefinitionNode node)
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

