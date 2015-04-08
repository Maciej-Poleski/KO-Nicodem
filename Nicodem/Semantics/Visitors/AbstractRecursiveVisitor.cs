using Nicodem.Semantics.AST;

namespace Nicodem.Semantics.Visitors
{
	abstract class AbstractRecursiveVisitor<TResult> : AbstractVisitor<TResult>
	{
		#region Node direct children
		public override TResult Visit(ProgramNode node)
		{
			base.Visit (node);
			foreach (var child in node.Functions)
				Visit (child);
		}

		public override TResult Visit(FunctionNode node)
		{
			base.Visit (node);
			Visit (node.Type);
			Visit (node.Body);
			foreach (var parameter in node.Parameters)
				Visit (parameter);
		}

		public override TResult Visit(ParameterNode node) 
		{
			base.Visit (node);
			Visit (node.Type);
		}

		public override TResult Visit(ExpressionNode node) 
		{
			base.Visit (node);
			Visit (node.ExpressionType);
		}
		#endregion

		#region TypeNode direct children
		public override TResult Visit(ArrayTypeNode node)
		{
			base.Visit (node);
			Visit (node.ElementType);
		}
		#endregion

		#region ExpressionNode direct children
		public override TResult Visit(ArrayNode node)
		{
			base.Visit (node);
			foreach (var child in node.Elements)
				Visit (child);
		}

		public override TResult Visit(BlockExpressionNode node)
		{
			base.Visit (node);
			foreach (var child in node.Elements)
				Visit (child);
		}

		public override TResult Visit(ConstNode node)
		{
			base.Visit (node);
			Visit (node.VariableType);
		}

		public override TResult Visit(ElementNode node)
		{
			base.Visit (node);
			Visit (node.Array);
			Visit (node.Index);
		}

		public override TResult Visit(FunctionCallNode node)
		{
			base.Visit (node);
			foreach (var arg in node.Arguments)
				Visit (arg);
		}

		public override TResult Visit(IfNode node)
		{
			base.Visit (node);
			Visit (node.Condition);
			Visit (node.Then);
			if (node.HasElse)
				Visit (node.Else);
		}

		public override TResult Visit(LoopControlNode node)
		{
			base.Visit (node);
			Visit (node.Value);
		}

		public override TResult Visit(OperationNode node)
		{
			base.Visit (node);
			foreach (var arg in node.Arguments)
				Visit (arg);
		}

		public override TResult Visit(SliceNode node)
		{
			base.Visit (node);
			Visit (node.Array);
			if (node.HasLeft)
				Visit (node.Left);
			if (node.HasRight)
				Visit (node.Right);
		}

		public override TResult Visit(VariableDefNode node)
		{
			base.Visit (node);
			Visit (node.VariableType);
			Visit (node.Value);
		}

		public override TResult Visit(VariableUseNode node)
		{
			base.Visit (node);
			Visit (node.Definition);
		}

		public override TResult Visit(WhileNode node)
		{
			base.Visit (node);
			Visit (node.Condition);
			Visit (node.Body);
			if (node.HasElse)
				Visit (node.Else);
		}
		#endregion
	}
}

