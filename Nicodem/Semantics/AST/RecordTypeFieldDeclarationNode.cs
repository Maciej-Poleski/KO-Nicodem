using Nicodem.Semantics.Visitors;
using Nicodem.Parser;
using System.Diagnostics;
using System;

namespace Nicodem.Semantics.AST
{
	class RecordTypeFieldDeclarationNode : Node, IComparable<RecordTypeFieldDeclarationNode>
	{
		public string Name { get; set; } // set during AST construction
		public TypeNode Type { get; set; } // set during AST construction

		#region implemented abstract members of Node

		// RecordFieldDefinition.SetProduction(ObjectDeclaration * ";");
		// ObjectDeclaration.SetProduction(TypeSpecifier * ObjectName);
		public override void BuildNode<TSymbol>(IParseTree<TSymbol> parseTree)
		{
			IParseTree<TSymbol>[] childs = ASTBuilder.ChildrenArray (parseTree);
			// type name
			Debug.Assert(childs.Length == 2); 
			Type = TypeNode.GetTypeNode(childs[0]); // type - arg 0
			Name = childs[1].Fragment.GetOriginText(); // name - arg 1
		}

		#endregion

		public RecordTypeFieldDeclarationNode() { }

		public RecordTypeFieldDeclarationNode(string name, TypeNode type)
		{
			this.Name = name;
			this.Type = type;
		}

		public override void Accept (AbstractVisitor visitor)
		{
			visitor.Visit (this);
		}

		public override T Accept<T>(ReturnedAbstractVisitor<T> visitor)
		{
			return visitor.Visit(this);
		}

		public int CompareTo (RecordTypeFieldDeclarationNode other)
		{
			return Name.CompareTo (other.Name);
		}

		protected override bool Compare(object rhs_)
		{
			var rhs = (RecordTypeFieldDeclarationNode)rhs_;
			return base.Compare(rhs) &&
				object.Equals(Name, rhs.Name) &&
				object.Equals(Type, rhs.Type);
		}

		protected override string PrintElements(string prefix)
		{
			return base.PrintElements(prefix)
				+ PrintVar(prefix, "Name", Name)
				+ PrintVar(prefix, "Type", Type);
		}
	}
}

