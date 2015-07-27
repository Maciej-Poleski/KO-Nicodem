using Nicodem.Semantics.AST;

namespace Nicodem.Semantics.Visitors
{
	class NameResolutionVisitor : AbstractRecursiveVisitor
	{
		readonly SymbolTable<VariableDeclNode> variableSymbolTable;
		readonly SymbolTable<FunctionDefinitionNode> functionSymbolTable;

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
			base.Visit (node);
		}

		public override void Visit (VariableUseNode node)
		{
			node.Declaration = variableSymbolTable.LookUp<VariableDeclNode> (node.Name);
			base.Visit (node);
		}

		public override void Visit (RecordVariableFieldUseNode node)
		{
			node.Definition = variableSymbolTable.LookUp<RecordVariableDefNode> (node.RecordName);
			base.Visit (node);
		}

        public override void Visit (FunctionDefinitionNode node)
        {
			variableSymbolTable.OpenBlock ();

            functionSymbolTable.Insert (node.Name, node);
            base.Visit (node);

			variableSymbolTable.CloseBlock ();
        }

        public override void Visit (FunctionCallNode node)
        {
			node.Definition = functionSymbolTable.LookUp<FunctionDefinitionNode> (node.Name);
            base.Visit (node);
        }
	}
}

