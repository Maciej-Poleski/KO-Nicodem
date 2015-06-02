using System;
using Nicodem.Semantics.Visitors;
using Nicodem.Parser;
using Nicodem.Semantics.ExpressionGraph;

namespace Nicodem.Semantics.AST
{
    /// <summary>
    /// Const node - parent class for ArrayNode and AtomNode.
    /// </summary>
	abstract class ConstNode : ExpressionNode
	{
        public TypeNode VariableType { get; private set; } // set during AST construction (in constructor)

        /// <value>Value of this constant.</value> 
        public string Value { get; set; } // set during AST construction TODO: store it as string?

        /// <summary>The only way to construct this node. Type cannot be unspecified.</summary>
		/// <param name="type">Type.</param>
		public ConstNode(TypeNode type)
		{
			this.VariableType = type;
		}
        
        #region implemented abstract members of Node

        public static ConstNode GetConstNode<TSymbol>(IParseTree<TSymbol> parseTree) where TSymbol:ISymbol<TSymbol>
        {
            // one of: DecimalNumberLiteral | CharacterLiteral | StringLiteral | BooleanLiteral
            TypeNode type;
            switch (ASTBuilder.GetName(parseTree.Symbol))
            {
                case "DecimalNumberLiteral":
                    type = NamedTypeNode.IntType();
                    break;
                case "CharacterLiteral":
                    type = NamedTypeNode.CharType();
                    break;
                case "StringLiteral":
                    throw new NotImplementedException(); // TODO: strings implementation
                case "BooleanLiteral":
                    type = NamedTypeNode.BoolType();
                    break;
                default:
                    throw new System.ArgumentException();
            }
            return new AtomNode(type);
        }

        public override void BuildNode<TSymbol>(IParseTree<TSymbol> parseTree)
        {
            throw new System.NotImplementedException();
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

