﻿using System;
using Nicodem.Semantics.AST;

namespace Nicodem.Semantics.Visitors
{
	class NameResolutionVisitor : AbstractRecursiveVisitor
	{
        private SymbolTable<VariableDeclNode> variableSymbolTable;
        private SymbolTable<FunctionDefinitionExpression> functionSymbolTable;

		public NameResolutionVisitor ()
		{
            variableSymbolTable = new SymbolTable<VariableDeclNode> ();
            functionSymbolTable = new SymbolTable<FunctionDefinitionExpression>();
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
            node.Declaration = variableSymbolTable.LookUp (node.Name);
			base.Visit (node);
		}

        public override void Visit (FunctionDefinitionExpression node)
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

