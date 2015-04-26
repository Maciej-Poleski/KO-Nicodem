using System.Collections.Generic;
using Nicodem.Semantics.Visitors;
using Nicodem.Parser;
using Nicodem.Backend;
using System.Linq;
using System.Diagnostics;

namespace Nicodem.Semantics.AST
{
	class FunctionDefinitionExpression : ExpressionNode
	{
		public string Name { get; set; }
        public IEnumerable<VariableDeclNode> Parameters { get; set; }
		public TypeNode ResultType { get; set; }
		public ExpressionNode Body { get; set; }
		public Function BackendFunction { get; set; }

        // ----- Constructor -----

        public FunctionDefinitionExpression(){
        }

        // ----- Methods -----

        #region implemented abstract members of Node

        private IEnumerable<VariableDeclNode> getParameters<TSymbol>(IParseTree<TSymbol> parseTree) where TSymbol:ISymbol<TSymbol> {
            var parList = new LinkedList<VariableDeclNode>();
            var node = ASTBuilder.AsBranch(parseTree);
            var childs = node.Children.GetEnumerator();
            while (childs.MoveNext()) {
                var par = new VariableDeclNode();
                par.BuildNode(childs.Current);
                parList.AddLast(par);
                childs.MoveNext(); // skip ',' if present
            }
            return parList;
        }

        private TypeNode getType<TSymbol>(IParseTree<TSymbol> parseTree) where TSymbol:ISymbol<TSymbol> {

            return null;
        }

        public override void BuildNode<TSymbol>(IParseTree<TSymbol> parseTree)
        {
            var node = ASTBuilder.AsBranch(parseTree);
            IParseTree<TSymbol>[] childs = node.Children.ToArray();
            // name ( params ) -> type expr
            Debug.Assert(childs.Length == 7); 
            // name from arg 0
            Name = childs[0].Fragment.GetOriginText();
            // parameters from arg 2
            Parameters = getParameters(childs[2]);
            // type from arg 5
            ResultType = TypeNode.GetTypeNode(childs[5]);
            // body from arg 6
            Body = ExpressionNode.GetExpressionNode(childs[6]);
        }

        #endregion


		public override void Accept (AbstractVisitor visitor)
		{
			visitor.Visit (this);
		}
	}
}

