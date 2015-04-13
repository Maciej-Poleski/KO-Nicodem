using System;
using Nicodem.Semantics.AST;

namespace Nicodem.Semantics.Visitors
{
	class NameResolutionVisitor : AbstractRecursiveVisitor
	{
		private SymbolTable symbolTable;

		public NameResolutionVisitor ()
		{
			symbolTable = new SymbolTable ();
		}

		public override void Visit (BlockExpressionNode node)
		{
			symbolTable.OpenBlock ();
			base.Visit (node);
			symbolTable.CloseBlock ();
		}

		public override void Visit (VariableDefNode node)
		{
			symbolTable.Insert (node.Name, node);
			base.Visit (node);
		}

		public override void Visit (VariableUseNode node)
		{
			node.Definition = symbolTable.LookUp (node.Name);
			base.Visit (node);
		}
	}
}

