using System.Collections.Generic;
using Nicodem.Semantics.Visitors;
using Nicodem.Parser;

namespace Nicodem.Semantics.AST
{
	class ProgramNode : Node
	{
        public IEnumerable<FunctionNode> Functions { get { return functions; } }

        private LinkedList<FunctionNode> functions;

        // ----- Constructor -----

        public ProgramNode(){
            functions = new LinkedList<FunctionNode>();
        }

        // ----- Methods -----
                
        #region implemented abstract members of Node

        public override void BuildNode<TSymbol>(IParseTree<TSymbol> parseTree)
        {
            var node = (ParseBranch<TSymbol>)parseTree;
            foreach(IParseTree<TSymbol> ch in node.Children){
                var funNode = new FunctionNode();
                funNode.BuildNode(ch);
                functions.AddLast(funNode);
            }
        }

        #endregion

		public override void Accept (AbstractVisitor visitor)
		{
			visitor.Visit (this);
		}
	}
}

