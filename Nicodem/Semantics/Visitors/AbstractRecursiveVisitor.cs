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
				Visit (child);
		}

		public override void Visit(FunctionNode node)
		{
			base.Visit (node);
			Visit (node.Type);
			Visit (node.Body);
			foreach (var parameter in node.Parameters)
				Visit (parameter);
		}

		public override void Visit(ParameterNode node) 
		{
			base.Visit (node);
			Visit (node.Type);
		}

		public override void Visit(ExpressionNode node) 
		{
			base.Visit (node);
			Visit (node.ExpressionType);
		}
		#endregion

		#region TypeNode direct children
		public override void Visit(ArrayTypeNode node)
		{
			base.Visit (node);
			Visit (node.ElementType);
		}
		#endregion

		#region ExpressionNode direct children
		public override void Visit(ArrayNode node)
		{
			base.Visit (node);
			foreach (var child in node.Elements)
				Visit (child);
		}

		public override void Visit(BlockExpressionNode node)
		{
			base.Visit (node);
			foreach (var child in node.Elements)
				Visit (child);
		}

		public override void Visit(ConstNode node)
		{
			base.Visit (node);
			Visit (node.VariableType);
		}

		public override void Visit(ElementNode node)
		{
			base.Visit (node);
			Visit (node.Array);
			Visit (node.Index);
		}

		public override void Visit(FunctionCallNode node)
		{
			base.Visit (node);
			foreach (var arg in node.Arguments)
				Visit (arg);
		}

		public override void Visit(IfNode node)
		{
			base.Visit (node);
			Visit (node.Condition);
			Visit (node.Then);
			if (node.HasElse)
				Visit (node.Else);
		}

		public override void Visit(LoopControlNode node)
		{
			base.Visit (node);
			Visit (node.Value);
		}

		public override void Visit(OperationNode node)
		{
			base.Visit (node);
			foreach (var arg in node.Arguments)
				Visit (arg);
		}

		public override void Visit(SliceNode node)
		{
			base.Visit (node);
			Visit (node.Array);
			if (node.HasLeft)
				Visit (node.Left);
			if (node.HasRight)
				Visit (node.Right);
		}

		public override void Visit(VariableDefNode node)
		{
			base.Visit (node);
			Visit (node.VariableType);
			Visit (node.Value);
		}

		public override void Visit(VariableUseNode node)
		{
			base.Visit (node);
			Visit (node.Definition);
		}

		public override void Visit(WhileNode node)
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

