using System.Collections.Generic;
using Nicodem.Semantics.Visitors;
using Nicodem.Parser;
using System.Linq;
using System.Diagnostics;

namespace Nicodem.Semantics.AST
{
	class FunctionNode : Node
	{
		public string Name { get; set; }
        public IEnumerable<ParameterNode> Parameters { get { return parameters; } }
		public TypeNode Type { get; set; }
		public ExpressionNode Body { get; set; }

        private LinkedList<ParameterNode> parameters;

        // ----- Constructor -----

        public FunctionNode(){
            parameters = new LinkedList<ParameterNode>();
            //Type = new TypeNode();
            //Body = new ExpressionNode();
        }

        // ----- Methods -----

        #region implemented abstract members of Node

        private void getParameters<TSymbol>(IParseTree<TSymbol> parseTree) where TSymbol:ISymbol<TSymbol> {
            var node = (ParseBranch<TSymbol>)parseTree;
            var childs = node.Children.GetEnumerator();
            while (childs.MoveNext()) {
                var par = new ParameterNode();
                par.BuildNode(childs.Current);
                parameters.AddLast(par);
                childs.MoveNext(); // skip ',' if present
            }
        }

        private TypeNode getType<TSymbol>(IParseTree<TSymbol> parseTree) where TSymbol:ISymbol<TSymbol> {
            return null;
        }

        private ExpressionNode getBody<TSymbol>(IParseTree<TSymbol> parseTree) where TSymbol:ISymbol<TSymbol> {
            return null;
        }

        public override void BuildNode<TSymbol>(IParseTree<TSymbol> parseTree)
        {
            var node = (ParseBranch<TSymbol>)parseTree;
            IParseTree<TSymbol>[] childs = node.Children.ToArray();
            Debug.Assert(childs.Length == 7); 
            // name ( params ) -> type expr

            // name from arg 0
            Name = childs[0].Fragment.GetOriginText();
            // parameters from arg 2
            getParameters(childs[2]);
            // type from arg 5
            Type = getType(childs[5]);
            // body from arg 6
            Body = getBody(childs[6]);
        }

        #endregion


		public override void Accept (AbstractVisitor visitor)
		{
			visitor.Visit (this);
		}
	}
}

