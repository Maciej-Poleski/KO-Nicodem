using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Nicodem.Semantics.Visitors;
using Nicodem.Parser;

namespace Nicodem.Semantics.AST
{
	abstract class TypeNode : Node
	{
		public bool IsConstant { get; set; }
        
        #region implemented abstract members of Node

        /// <summary>
        /// Resolve type defined by this typeExpression. Type Expression must be given in reversed order.
        /// </summary>
        /// <returns>The resolved type.</returns>
        /// <param name="typeExpr">Enumerator of reversed type expresion enumeration.</param>
        public static TypeNode RecursiveResolveArrayType<TSymbol>(IEnumerator<IParseTree<TSymbol>> typeExpr) where TSymbol:ISymbol<TSymbol>
        {
            bool isMutable = ASTBuilder.EatSymbol("mutable", typeExpr);
            if (ASTBuilder.EatSymbol("]", typeExpr)) { // if ] is present then create ArrayType
                var resType = new ArrayTypeNode();
                resType.IsConstant = !isMutable;
                resType.IsFixedSize = true; // TODO: what about dynamic arrays?
                resType.Length = ExpressionNode.EvalExpression(typeExpr.Current);
                typeExpr.MoveNext();
                Debug.Assert(ASTBuilder.EatSymbol("[", typeExpr));
                resType.ElementType = RecursiveResolveArrayType(typeExpr);
                return resType;
            } else {
                var resType = new NamedTypeNode();
                resType.BuildNode(typeExpr.Current);
                resType.IsConstant = !isMutable;
                return resType;
            }
        }

        // TypeSpecifier -> (TypeName * ("mutable".Optional() * "\\[" * Expression * "\\]").Star * "mutable".Optional());
        public static TypeNode GetTypeNode<TSymbol>(IParseTree<TSymbol> parseTree) where TSymbol:ISymbol<TSymbol>
        {
            var node = (ParseBranch<TSymbol>)parseTree;
            var childs = node.Children.Reverse().GetEnumerator();
            Debug.Assert(childs.MoveNext());
            return RecursiveResolveArrayType(childs);
        }

        #endregion

		public override void Accept (AbstractVisitor visitor)
		{
			visitor.Visit (this);
		}
	}
}

