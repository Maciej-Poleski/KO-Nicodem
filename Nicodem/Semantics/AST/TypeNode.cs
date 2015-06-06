using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Nicodem.Semantics.Visitors;
using Nicodem.Parser;
using Nicodem.Semantics.ExpressionGraph;

namespace Nicodem.Semantics.AST
{
	abstract class TypeNode : Node
	{
        public bool IsConstant { get; set; } // set during AST construction
        
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
                resType.IsFixedSize = false; // we initialize all arrays as dynamic size
                if (ASTBuilder.EatSymbol("[", typeExpr)) { // opening bracket, no length expression
                    resType.LengthExpression = null;
                } else { // length expression present
                    resType.LengthExpression = ExpressionNode.GetExpressionNode(typeExpr.Current);
                    typeExpr.MoveNext();
                    Debug.Assert(ASTBuilder.EatSymbol("[", typeExpr)); // eat opening bracket
                }
                resType.ElementType = RecursiveResolveArrayType(typeExpr);
                return resType;
            } else {
                var resType = new NamedTypeNode();
                resType.BuildNode(typeExpr.Current);
                resType.IsConstant = !isMutable;
                return resType;
            }
        }

        // TypeSpecifier -> (TypeName * ("mutable".Optional() * "\\[" * Expression.Optional * "\\]").Star * "mutable".Optional());
        public static TypeNode GetTypeNode<TSymbol>(IParseTree<TSymbol> parseTree) where TSymbol:ISymbol<TSymbol>
        {
            var node = ASTBuilder.AsBranch(parseTree);
            var childs = node.Children.Reverse().GetEnumerator();
            Debug.Assert(childs.MoveNext());
            return RecursiveResolveArrayType(childs);
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

        //public static bool Compare(TypeNode a, TypeNode b){
        //    if (a == null) 
        //        return false;
        //    if (b == null) 
        //        return false;
        //    if (!a.GetType().Equals(b.GetType()))
        //        return false;
        //    if (a is ArrayTypeNode && b is ArrayTypeNode)
        //    {
        //        if (TypeNode.Compare(((ArrayTypeNode)a).ElementType, ((ArrayTypeNode)b).ElementType))
        //            return true;
        //    }
        //    if (a is NamedTypeNode && b is NamedTypeNode)
        //    {
        //        if (((NamedTypeNode)a).Name.Equals(((NamedTypeNode)b).Name))
        //            return true;
        //    }
        //    return false;
        //}
        

        protected override bool Compare(object rhs_)
        {
            var rhs = (TypeNode)rhs_;
            return base.Compare(rhs) &&
                IsConstant.Equals(rhs.IsConstant);
        }

        protected override string PrintElements(string prefix)
        {
            return base.PrintElements(prefix) +
                PrintVar(prefix, "IsConstant", IsConstant);
        }
	}
}

