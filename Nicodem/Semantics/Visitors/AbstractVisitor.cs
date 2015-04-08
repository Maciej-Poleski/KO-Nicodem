using Nicodem.Semantics.AST;

namespace Nicodem.Semantics.Visitors
{
	abstract class AbstractVisitor<TResult>
	{
		/// <summary>
		/// 	Every visitor should implement this method to define default result.
		/// </summary>
		/// <param name="node">Node.</param>
		public abstract TResult Visit (Node node);

		#region Node direct children
		public virtual TResult Visit(ProgramNode node) {
			return Visit (node as Node);
		}

		public virtual TResult Visit(FunctionNode node) {
			return Visit (node as Node);
		}

		public virtual TResult Visit(ParameterNode node) {
			return Visit (node as Node);
		}

		public virtual TResult Visit(TypeNode node) {
			return Visit (node as Node);
		}

		public virtual TResult Visit(ExpressionNode node) {
			return Visit (node as Node);
		}
		#endregion


		#region TypeNode direct children
		public virtual TResult Visit(NamedTypeNode node) {
			return Visit (node as TypeNode);
		}
		public virtual TResult Visit(ArrayTypeNode node) {
			return Visit (node as TypeNode);
		}
		#endregion

		#region ExpressionNode direct children
		public virtual TResult Visit(ArrayNode node) {
			return Visit (node as ConstNode);
		}

		public virtual TResult Visit(BlockExpressionNode node) {
			return Visit (node as ExpressionNode);
		}

		public virtual TResult Visit(ConstNode node) {
			return Visit (node as ExpressionNode);
		}

		public virtual TResult Visit(ElementNode node) {
			return Visit (node as ExpressionNode);
		}

		public virtual TResult Visit(FunctionCallNode node) {
			return Visit (node as ExpressionNode);
		}

		public virtual TResult Visit(IfNode node) {
			return Visit (node as ExpressionNode);
		}

		public virtual TResult Visit(LoopControlNode node) {
			return Visit (node as ExpressionNode);
		}

		public virtual TResult Visit(OperationNode node) {
			return Visit (node as ExpressionNode);
		}

		public virtual TResult Visit(SliceNode node) {
			return Visit (node as ExpressionNode);
		}

		public virtual TResult Visit(VariableDefNode node) {
			return Visit (node as ExpressionNode);
		}

		public virtual TResult Visit(VariableUseNode node) {
			return Visit (node as ExpressionNode);
		}

		public virtual TResult Visit(WhileNode node) {
			return Visit (node as ExpressionNode);
		}
		#endregion

		public virtual TResult Visit(OperatorNode node) {
			return Visit (node as OperationNode);
		}
	}
}

