using System.Collections.Generic;
using Nicodem.Semantics.Visitors;
using Nicodem.Parser;
using Nicodem.Semantics.ExpressionGraph;

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

        public override void BuildNode<TSymbol>(IParseTree<TSymbol> parseTree)
        {
            var node = ASTBuilder.AsBranch(parseTree);
            foreach(IParseTree<TSymbol> ch in node.Children){
                var funNode = new FunctionDefinitionNode();
                funNode.BuildNode(ch);
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
	}
}

