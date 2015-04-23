using Nicodem.Semantics.AST;

namespace Nicodem.Semantics.Visitors
{
	abstract class AbstractRecursiveVisitor : AbstractVisitor
	{
		#region Node direct children
		public override void Visit(ProgramNode node)
		{
			base.Visit (node);
			foreach (var child in node.Functions)
				child.Accept (this);
		}

		public override void Visit(FunctionNode node)
		{
			base.Visit (node);
			node.Type.Accept (this);
			node.Body.Accept (this);
			foreach (var parameter in node.Parameters)
				parameter.Accept (this);
		}

		public override void Visit(ExpressionNode node) 
		{
			base.Visit (node);
			node.ExpressionType.Accept (this);
		}
		#endregion

		#region TypeNode direct children
		public override void Visit(ArrayTypeNode node)
		{
			base.Visit (node);
			node.ElementType.Accept (this);
		}
		#endregion

		#region ExpressionNode direct children
		public override void Visit(ArrayNode node)
		{
			base.Visit (node);
			foreach (var child in node.Elements)
				child.Accept (this);
		}

        public override void Visit(AtomNode node)
        {
            base.Visit(node);
            node.VariableType.Accept(this);
        }

		public override void Visit(BlockExpressionNode node)
		{
			base.Visit (node);
			foreach (var child in node.Elements)
				child.Accept (this);
		}

		public override void Visit(ElementNode node)
		{
			base.Visit (node);
			node.Array.Accept (this);
			node.Index.Accept (this);
		}

		public override void Visit(FunctionCallNode node)
		{
			base.Visit (node);
			foreach (var arg in node.Arguments)
				arg.Accept (this);
		}

		public override void Visit(IfNode node)
		{
			base.Visit (node);
			node.Condition.Accept (this);
			node.Then.Accept (this);
			if (node.HasElse)
				node.Else.Accept (this);
		}

		public override void Visit(LoopControlNode node)
		{
			base.Visit (node);
			node.Value.Accept (this);
		}

		public override void Visit(OperationNode node)
		{
			base.Visit (node);
			foreach (var arg in node.Arguments)
				arg.Accept (this);
		}

		public override void Visit(SliceNode node)
		{
			base.Visit (node);
			node.Array.Accept (this);
			if (node.HasLeft)
				node.Left.Accept (this);
			if (node.HasRight)
				node.Right.Accept (this);
		}

        public override void Visit(VariableDeclNode node)
        {
            base.Visit(node);
            node.Type.Accept(this);
        }

        #region VariableDeclNode direct children

        public override void Visit(VariableDefNode node)
		{
			base.Visit (node);
			node.Value.Accept (this);
		}

        #endregion

		public override void Visit(VariableUseNode node)
		{
			base.Visit (node);
			node.Declaration.Accept (this);
		}

		public override void Visit(WhileNode node)
		{
			base.Visit (node);
			node.Condition.Accept (this);
			node.Body.Accept (this);
			if (node.HasElse)
				node.Else.Accept (this);
		}
		#endregion
	}
}

