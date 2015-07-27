using System.Collections.Generic;
using Nicodem.Semantics.Visitors;
using Nicodem.Parser;

namespace Nicodem.Semantics.AST
{
	class ProgramNode : Node
	{
        public IEnumerable<FunctionDefinitionNode> Functions { get { return functions; } }
		public IEnumerable<RecordTypeDeclarationNode> Records { get { return records; } }

        private LinkedList<FunctionDefinitionNode> functions; // set during AST construction
		private LinkedList<RecordTypeDeclarationNode> records; // set during AST construction

        // ----- Constructor -----

        public ProgramNode(){
            functions = new LinkedList<FunctionDefinitionNode>();
			records = new LinkedList<RecordTypeDeclarationNode>();
        }

		public ProgramNode(LinkedList<FunctionDefinitionNode> functions, LinkedList<RecordTypeDeclarationNode> records)
		{
			this.functions = functions;
			this.records = records;
		}

        // ----- Methods -----
                
        #region implemented abstract members of Node

		// Program -> (Function|Record)* Eof
        public override void BuildNode<TSymbol>(IParseTree<TSymbol> parseTree)
        {
            var childs = ASTBuilder.ChildrenArray(parseTree);
            for (int i=0; i<childs.Length-1; i++) { // skip last child - it is EOF
				switch (ASTBuilder.GetName (childs [i].Symbol)) {

				case "RecordTypeDeclaration":
					var recordNode = new RecordTypeDeclarationNode ();
					recordNode.BuildNode (childs [i]);
					records.AddLast (recordNode);
					break;
				
				default:
					var funNode = new FunctionDefinitionNode ();
					funNode.BuildNode (childs [i]);
					functions.AddLast (funNode);
					break;
				}
            }
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
            var rhs = (ProgramNode)rhs_;
            return base.Compare(rhs) &&
                SequenceEqual(Functions, rhs.Functions) &&
				SequenceEqual(Records, rhs.Records);
        }

        protected override string PrintElements(string prefix)
        {
            return base.PrintElements(prefix)
                + PrintVar(prefix, "Functions", Functions)
				+ PrintVar(prefix, "Records", Records);
        }
	}
}

