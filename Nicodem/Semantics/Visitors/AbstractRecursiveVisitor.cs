using Nicodem.Semantics.AST;

namespace Nicodem.Semantics.Visitors
{
	abstract class AbstractRecursiveVisitor : AbstractVisitor
	{
		#region Node direct children
		public override void Visit(ProgramNode node)
		{
			base.Visit (node);

			if (!ReferenceEquals(node.Functions, null))
				foreach (var child in node.Functions)
					if (!ReferenceEquals(child, null))
						child.Accept (this);
		}

		public override void Visit(ExpressionNode node) 
		{
			base.Visit (node);
			if (!ReferenceEquals(node.ExpressionType, null))
				node.ExpressionType.Accept (this);
		}
		#endregion

		#region TypeNode direct children
		public override void Visit(ArrayTypeNode node)
		{
			base.Visit (node);
			if (!ReferenceEquals(node.ElementType, null))
				node.ElementType.Accept (this);
		}
		#endregion

        #region ExpressionNode direct children

        public override void Visit(FunctionDefinitionExpression node)
        {
            base.Visit(node);

			if (!ReferenceEquals(node.Parameters, null))
				foreach (var parameter in node.Parameters)
					if (!ReferenceEquals(parameter, null))
						parameter.Accept(this);

			if (!ReferenceEquals(node.ResultType, null))
				node.ResultType.Accept(this);

			if (!ReferenceEquals(node.Body, null))
				node.Body.Accept(this);
        }

		public override void Visit(ArrayNode node)
		{
			base.Visit (node);

			if (!ReferenceEquals(node.Elements, null))
				foreach (var child in node.Elements)
					if (!ReferenceEquals(child, null))
						child.Accept (this);
		}

        public override void Visit(AtomNode node)
        {
            base.Visit(node);
			if (!ReferenceEquals(node.VariableType, null))
				node.VariableType.Accept(this);
        }

		public override void Visit(BlockExpressionNode node)
		{
			base.Visit (node);

			if (!ReferenceEquals(node.Elements, null))
				foreach (var child in node.Elements)
					if (!ReferenceEquals(child, null))
						child.Accept (this);
		}

		public override void Visit(ElementNode node)
		{
			base.Visit (node);

			if (!ReferenceEquals(node.Array, null))
				node.Array.Accept (this);

			if (!ReferenceEquals(node.Index, null))
				node.Index.Accept (this);
		}

		public override void Visit(FunctionCallNode node)
		{
			base.Visit (node);

			if (!ReferenceEquals(node.Arguments, null))
				foreach (var arg in node.Arguments)
					if (!ReferenceEquals(arg, null))
						arg.Accept (this);
		}

		public override void Visit(IfNode node)
		{
			base.Visit (node);

			if (!ReferenceEquals(node.Condition, null))
				node.Condition.Accept (this);

			if (!ReferenceEquals(node.Then, null))
				node.Then.Accept (this);

			if (node.HasElse)
				node.Else.Accept (this);
		}

		public override void Visit(LoopControlNode node)
		{
			base.Visit (node);
			if (!ReferenceEquals(node.Value, null))
				node.Value.Accept (this);
		}

		public override void Visit(OperationNode node)
		{
			base.Visit (node);

			if (!ReferenceEquals(node.Arguments, null))
				foreach (var arg in node.Arguments)
					if (!ReferenceEquals(arg, null))
						arg.Accept (this);
		}

		public override void Visit(SliceNode node)
		{
			base.Visit (node);

			if (!ReferenceEquals(node.Array, null))
				node.Array.Accept (this);

			if (node.HasLeft)
				node.Left.Accept (this);

			if (node.HasRight)
				node.Right.Accept (this);
		}

        public override void Visit(VariableDeclNode node)
        {
            base.Visit(node);
			if (!ReferenceEquals(node.Type, null))
				node.Type.Accept(this);
        }

        #region VariableDeclNode direct children

        public override void Visit(VariableDefNode node)
		{
			base.Visit (node);
			if (!ReferenceEquals(node.Value, null))
				node.Value.Accept (this);
		}

        #endregion

		public override void Visit(WhileNode node)
		{
			base.Visit (node);

			if (!ReferenceEquals(node.Condition, null))
				node.Condition.Accept (this);

			if (!ReferenceEquals(node.Body, null))
				node.Body.Accept (this);

			if (node.HasElse)
				node.Else.Accept (this);
		}
		#endregion
	}
}

