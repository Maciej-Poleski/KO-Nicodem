using Nicodem.Parser;
using Nicodem.Semantics.Visitors;

namespace Nicodem.Semantics.AST
{
	class RecordVariableFieldUseNode : ExpressionNode
	{
		public string RecordName { get; set; } // AST
		public string Field { get; set; }  // AST
		public RecordVariableFieldDefNode Definition { get; set; } // name resolution

		#region implemented abstract members of Node

		public override void BuildNode<TSymbol> (IParseTree<TSymbol> parseTree)
		{
			// <recordName> [ <field> ]
			var children = ASTBuilder.ChildrenArray(parseTree);
			RecordName = children [0].Fragment.GetOriginText ();
			Field = children [2].Fragment.GetOriginText ();
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

		protected override bool Compare(object rhs_)
		{
			var rhs = (RecordVariableFieldUseNode)rhs_;
			return base.Compare(rhs) &&
				object.Equals(RecordName, rhs.RecordName) &&
				object.Equals(Field, rhs.Field);
		}

		protected override string PrintElements(string prefix)
		{
			return base.PrintElements(prefix)
				+ PrintVar(prefix, "RecordName", RecordName)
				+ PrintVar(prefix, "Field", Field);
		}
	}
}

