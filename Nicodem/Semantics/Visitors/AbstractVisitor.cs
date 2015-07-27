using Nicodem.Semantics.AST;

namespace Nicodem.Semantics.Visitors
{
	abstract class AbstractVisitor
	{
		public virtual void Visit (Node node)
		{
		}

		#region Node direct children
		public virtual void Visit(ProgramNode node) {
			Visit (node as Node);
		}

		public virtual void Visit(RecordTypeDeclarationNode node) {
			Visit (node as Node);
		}

		public virtual void Visit(RecordTypeFieldDeclarationNode node) {
			Visit (node as Node);
		}

		public virtual void Visit(RecordVariableFieldDefNode node) {
			Visit (node as Node);
		}

		public virtual void Visit(TypeNode node) {
			Visit (node as Node);
		}

		public virtual void Visit(ExpressionNode node) {
			Visit (node as Node);
		}
		#endregion

		#region TypeNode direct children
		public virtual void Visit(NamedTypeNode node) {
			Visit (node as TypeNode);
		}
		public virtual void Visit(ArrayTypeNode node) {
			Visit (node as TypeNode);
		}
		#endregion

        #region ExpressionNode direct children

        public virtual void Visit(FunctionDefinitionNode node)
        {
            Visit(node as ExpressionNode);
        }

		public virtual void Visit(ArrayNode node) {
			Visit (node as ConstNode);
		}

        public virtual void Visit(AtomNode node)
        {
            Visit(node as ExpressionNode);
        }

		public virtual void Visit(BlockExpressionNode node) {
			Visit (node as ExpressionNode);
		}

		public virtual void Visit(ElementNode node) {
			Visit (node as ExpressionNode);
		}

		public virtual void Visit(FunctionCallNode node) {
			Visit (node as ExpressionNode);
		}

		public virtual void Visit(IfNode node) {
			Visit (node as ExpressionNode);
		}

		public virtual void Visit(LoopControlNode node) {
			Visit (node as ExpressionNode);
		}

		public virtual void Visit(SliceNode node) {
			Visit (node as ExpressionNode);
		}

        public virtual void Visit(VariableDeclNode node)
        {
            Visit(node as ExpressionNode);
        }

	    #region VariableDeclNode direct children

        public virtual void Visit(VariableDefNode node)
        {
            Visit(node as VariableDeclNode);
        }

		#region VariableDefNode direct children

		public virtual void Visit(RecordVariableDefNode node) 
		{
			Visit (node as VariableDefNode);
		}

		#endregion

	    #endregion

		public virtual void Visit(VariableUseNode node) {
			Visit (node as ExpressionNode);
		}

		public virtual void Visit(RecordVariableFieldUseNode node) {
			Visit (node as ExpressionNode);
		}

		public virtual void Visit(WhileNode node) {
			Visit (node as ExpressionNode);
		}
		#endregion

		public virtual void Visit(OperatorNode node) {
			Visit (node as ExpressionNode);
		}
	}
}

