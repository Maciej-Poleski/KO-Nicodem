using System;
using Nicodem.Parser;
using Nicodem.Semantics.Visitors;

namespace Nicodem.Semantics.AST
{
	class RecordVariableFieldDefNode : Node, IComparable<RecordVariableFieldDefNode>
	{
		public string FieldName { get; set; }
		public ExpressionNode Value { get; set; }

		#region implemented abstract members of Node

		public override void BuildNode<TSymbol> (IParseTree<TSymbol> parseTree)
		{
			var arr = ASTBuilder.ChildrenArray (parseTree);
			FieldName = arr [0].Fragment.GetOriginText ();
			Value = ExpressionNode.GetExpressionNode (arr [2]);
		}

		#endregion

		public override void Accept (AbstractVisitor visitor)
		{
			visitor.Visit (this);
		}

		public override T Accept<T>(ReturnedAbstractVisitor<T> visitor)
		{
			return visitor.Visit(this);
		}

		public int CompareTo (RecordVariableFieldDefNode other)
		{
			return FieldName.CompareTo (other.FieldName);
		}

		protected override bool Compare(object rhs_)
		{
			var rhs = (RecordVariableFieldDefNode)rhs_;
			return base.Compare(rhs) &&
				object.Equals(FieldName, rhs.FieldName) &&
				object.Equals(Value, rhs.Value);
		}

		protected override string PrintElements(string prefix)
		{
			return base.PrintElements(prefix)
				+ PrintVar(prefix, "FieldName", FieldName)
				+ PrintVar(prefix, "Value", Value);
		}
	}
}

