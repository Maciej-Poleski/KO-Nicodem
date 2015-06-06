using System.Collections.Generic;
using System.Diagnostics;
using Nicodem.Semantics.Visitors;
using Nicodem.Parser;
using Nicodem.Semantics.ExpressionGraph;
using System;
using System.Linq;

namespace Nicodem.Semantics.AST
{
	class ProgramNode : Node
	{
        public IEnumerable<FunctionDefinitionNode> Functions { get { return functions; } }

        private LinkedList<FunctionDefinitionNode> functions; // set during AST construction

        // ----- Constructor -----

        public ProgramNode(){
            functions = new LinkedList<FunctionDefinitionNode>();
        }

	    public ProgramNode(LinkedList<FunctionDefinitionNode> functions)
	    {
	        this.functions = functions;
	    }

        // ----- Methods -----
                
        #region implemented abstract members of Node

        // Program -> Function* Eof
        public override void BuildNode<TSymbol>(IParseTree<TSymbol> parseTree)
        {
            var childs = ASTBuilder.ChildrenArray(parseTree);
            for (int i=0; i<childs.Length-1; i++) { // skip last child - it is EOF
                var funNode = new FunctionDefinitionNode();
                funNode.BuildNode(childs[i]);
                functions.AddLast(funNode);
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
                SequenceEqual(Functions, rhs.Functions);
        }

        protected override string PrintElements(string prefix)
        {
            return base.PrintElements(prefix)
                + PrintVar(prefix, "Functions", Functions);
        }
	}
}

