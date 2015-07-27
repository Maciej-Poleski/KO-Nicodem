using Nicodem.Semantics.AST;

namespace Nicodem.Semantics.Visitors
{
    class ReturnedAbstractVisitor<T>
    {
        public virtual T Visit(Node node)
        {
            return default(T);
        }

        #region Node direct children
        public virtual T Visit(ProgramNode node)
        {
            return Visit(node as Node);
        }

		public virtual T Visit(RecordTypeDeclarationNode node) 
		{
			return Visit(node as Node);
		}

		public virtual T Visit(RecordTypeFieldDeclarationNode node) 
		{
			return Visit(node as Node);
		}

		public virtual T Visit(RecordVariableFieldDefNode node) 
		{
			return Visit(node as Node);
		}

        public virtual T Visit(TypeNode node)
        {
            return Visit(node as Node);
        }

        public virtual T Visit(ExpressionNode node)
        {
            return Visit(node as Node);
        }
        #endregion

        #region TypeNode direct children
        public virtual T Visit(NamedTypeNode node)
        {
           return Visit(node as TypeNode);
        }
        public virtual T Visit(ArrayTypeNode node)
        {
           return Visit(node as TypeNode);
        }
        #endregion

        #region ExpressionNode direct children

        public virtual T Visit(FunctionDefinitionNode node)
        {
           return Visit(node as ExpressionNode);
        }

        public virtual T Visit(ArrayNode node)
        {
           return Visit(node as ConstNode);
        }

        public virtual T Visit(AtomNode node)
        {
           return Visit(node as ExpressionNode);
        }

        public virtual T Visit(BlockExpressionNode node)
        {
           return Visit(node as ExpressionNode);
        }

        public virtual T Visit(ElementNode node)
        {
           return Visit(node as ExpressionNode);
        }

        public virtual T Visit(FunctionCallNode node)
        {
           return Visit(node as ExpressionNode);
        }

        public virtual T Visit(IfNode node)
        {
           return Visit(node as ExpressionNode);
        }

        public virtual T Visit(LoopControlNode node)
        {
           return Visit(node as ExpressionNode);
        }

        public virtual T Visit(SliceNode node)
        {
           return Visit(node as ExpressionNode);
        }

        public virtual T Visit(VariableDeclNode node)
        {
           return Visit(node as ExpressionNode);
        }

        #region VariableDeclNode direct children

        public virtual T Visit(VariableDefNode node)
        {
           return Visit(node as VariableDeclNode);
        }

		#region VariableDefNode direct children

		public virtual T Visit(RecordVariableDefNode node)
		{
			return Visit(node as VariableDefNode);
		}

		#endregion

        #endregion

        public virtual T Visit(VariableUseNode node)
        {
           return Visit(node as ExpressionNode);
        }

		public virtual T Visit(RecordVariableFieldUseNode node)
		{
			return Visit(node as ExpressionNode);
		}

        public virtual T Visit(WhileNode node)
        {
           return Visit(node as ExpressionNode);
        }
        #endregion

        public virtual T Visit(OperatorNode node)
        {
           return Visit(node as ExpressionNode);
        }
    }
}
