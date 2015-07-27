using System;
using Nicodem.Parser;
using System.Collections.Generic;
using Nicodem.Semantics.Visitors;
using System.Diagnostics;

namespace Nicodem.Semantics.AST
{
	/// <summary>
	/// Declaration of new record type
	/// <Name> {
	/// 	<Field>+
	/// }
	/// </summary>
	class RecordTypeDeclarationNode : Node
	{
		public TypeNode Name { get; set; } // set during AST construction
		public IEnumerable<RecordTypeFieldDeclarationNode> Fields { get; set; } // set during AST construction

		public RecordTypeDeclarationNode(string name, IEnumerable<RecordTypeFieldDeclarationNode> fields) {
			this.Name = new NamedTypeNode { Name = name, IsConstant = false };
			this.Fields = fields;
		}

		public RecordTypeDeclarationNode()
			: this (string.Empty, new LinkedList<RecordTypeFieldDeclarationNode>())
		{}

		#region implemented abstract members of Node

		static IEnumerable<RecordTypeFieldDeclarationNode> GetFields<TSymbol>(IParseTree<TSymbol> parseTree) 
			where TSymbol:ISymbol<TSymbol> 
		{
			var lst = new LinkedList<RecordTypeFieldDeclarationNode>();
			var childs = ASTBuilder.ChildrenEnumerator(parseTree);
			while (childs.MoveNext()) {
				var objectDefinition = ASTBuilder.ChildrenArray (childs.Current) [0];
				var fld = new RecordTypeFieldDeclarationNode ();
				fld.BuildNode (objectDefinition);
				lst.AddLast (fld);
			}
			return lst;
		}


		public override void BuildNode<TSymbol> (IParseTree<TSymbol> parseTree)
		{
			Console.WriteLine ("Build ast: " + parseTree);
			IParseTree<TSymbol>[] childs = ASTBuilder.ChildrenArray (parseTree);
			// name { fields }
			Debug.Assert(childs.Length == 4); 
			// name from arg 0
			Name = TypeNode.GetTypeNode(childs[0]);
			// fields from arg 2
			Fields = GetFields(childs[2]);
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
			var rhs = (RecordTypeDeclarationNode)rhs_;
			return base.Compare(rhs) &&
				object.Equals(Name, rhs.Name) &&
				SequenceEqual(Fields, rhs.Fields);
		}

		protected override string PrintElements(string prefix)
		{
			return base.PrintElements(prefix) +
				PrintVar(prefix, "Name", Name) +
				PrintVar(prefix, "Fields", Fields);
		}
	}
}

