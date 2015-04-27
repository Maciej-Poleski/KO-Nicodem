using System.Collections.Generic;
using Nicodem.Semantics.Visitors;
using Nicodem.Parser;

namespace Nicodem.Semantics.AST
{
	class ProgramNode : Node
	{
        public IEnumerable<FunctionDefinitionExpression> Functions { get { return functions; } }

        private LinkedList<FunctionDefinitionExpression> functions; // set during AST construction

        // ----- Constructor -----

        public ProgramNode(){
            functions = new LinkedList<FunctionDefinitionExpression>();
        }

	    public ProgramNode(LinkedList<FunctionDefinitionExpression> functions)
	    {
	        this.functions = functions;
	    }

        // ----- Methods -----
                
        #region implemented abstract members of Node

        public override void BuildNode<TSymbol>(IParseTree<TSymbol> parseTree)
        {
            var node = ASTBuilder.AsBranch(parseTree);
            foreach(IParseTree<TSymbol> ch in node.Children){
                var funNode = new FunctionDefinitionExpression();
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

