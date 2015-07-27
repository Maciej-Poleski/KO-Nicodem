using Nicodem.Parser;
using Nicodem.Semantics.Visitors;
using System.Diagnostics;
using System.Collections.Generic;

namespace Nicodem.Semantics.AST
{
	/// <summary>
	/// Definition of new record-type variable
	/// </summary>
	class RecordVariableDefNode : VariableDefNode
	{
		public IEnumerable<RecordVariableFieldDefNode> Fields { get; set; } // set during AST construction

		#region implemented abstract members of Node

		public IEnumerable<RecordVariableFieldDefNode> GetFields<TSymbol> (IParseTree<TSymbol> parseTree) where TSymbol:ISymbol<TSymbol>  {
			var lst = new LinkedList<RecordVariableFieldDefNode> ();
			var childs = ASTBuilder.ChildrenEnumerator(parseTree);
			while (childs.MoveNext()) {
				var v = new RecordVariableFieldDefNode ();
				v.BuildNode(childs.Current);
				lst.AddLast(v);
				childs.MoveNext(); // skip ',' if present
			}
			return lst;
		}

		public override void BuildNode<TSymbol> (IParseTree<TSymbol> parseTree)
		{
			// <type> <name> { <fields> }
			var children = ASTBuilder.ChildrenArray(parseTree);
			Debug.Assert(children.Length == 5);

			Type = TypeNode.GetTypeNode(children[0]); // type - arg 0
			Name = children[1].Fragment.GetOriginText(); // name - arg 1
			Fields = GetFields(children [3]); // fields - arg 3
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
			var rhs = (RecordVariableDefNode)rhs_;
			return base.Compare(rhs) &&
				object.Equals(Name, rhs.Name) &&
				object.Equals(Type, rhs.Type) &&
				object.Equals(Fields, rhs.Fields) &&
				object.Equals(NestedUse, rhs.NestedUse) &&
				object.Equals(VariableLocation, rhs.VariableLocation);
		}

		protected override string PrintElements(string prefix)
		{
			return base.PrintElements(prefix)
				+ PrintVar(prefix, "Fields", Fields);
		}
	}
}

