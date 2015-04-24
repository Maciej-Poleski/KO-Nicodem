using System;
using System.Collections.Generic;
using Nicodem.Semantics.AST;

namespace Nicodem.Semantics.Visitors
{
    /// <summary>
    ///     Names of all variables must be resolved before use of this visitor.
    /// </summary>
    internal class NestedUseVisitor : AbstractVisitor
    {
        public override void Visit(Node node)
        {
            throw new InvalidOperationException("NestedUseVisitor can be used only on root of AST tree (ProgramNode)");
        }

        public override void Visit(ProgramNode node)
        {
            var visitor1 = new NestedUseVisitor1();
            node.Accept(visitor1);
            var visitor2 = new NestedUseVisitor2(visitor1.DeclToFunction);
            node.Accept(visitor2);
        }
    }

    internal class NestedUseVisitor1 : AbstractRecursiveVisitor
    {
        private readonly Dictionary<VariableDeclNode, FunctionDefinitionExpression> _declToFunction =
            new Dictionary<VariableDeclNode, FunctionDefinitionExpression>();

        private FunctionDefinitionExpression _currentFunction;

        internal IReadOnlyDictionary<VariableDeclNode, FunctionDefinitionExpression> DeclToFunction
        {
            get { return _declToFunction; }
        }

        public override void Visit(FunctionDefinitionExpression node)
        {
            _currentFunction = node;
            base.Visit(node);
        }

        public override void Visit(VariableDeclNode node)
        {
            if (_currentFunction == null)
            {
                throw new NotSupportedException("Variable declaration outside of function is currently not supported");
            }
            _declToFunction[node] = _currentFunction;
            base.Visit(node);
        }
    }

    /// <summary>
    ///     Names of all variables must be resolved before use of this visitor.
    /// </summary>
    internal class NestedUseVisitor2 : AbstractRecursiveVisitor
    {
        private readonly IReadOnlyDictionary<VariableDeclNode, FunctionDefinitionExpression> _declToFunction;
        private FunctionDefinitionExpression _currentFunction;

        public NestedUseVisitor2(IReadOnlyDictionary<VariableDeclNode, FunctionDefinitionExpression> declToFunction)
        {
            _declToFunction = declToFunction;
        }

        public override void Visit(FunctionDefinitionExpression node)
        {
            _currentFunction = node;
            base.Visit(node);
        }

        public override void Visit(VariableUseNode node)
        {
            Visit(node as ExpressionNode);
            if (_declToFunction[node.Declaration] != _currentFunction)
            {
                node.Declaration.NestedUse = true;
            }
            // WARNING: AbstractRecursiveVisitor traverses all edges (including non-tree)
        }
    }

    public static partial class Extensions
    {
        /// <summary>
        ///     Names of all variables must be resolved before use of this visitor.
        /// </summary>
        /// <param name="node">Root of AST tree</param>
        internal static void FillInNestedUseFlag(this ProgramNode node)
        {
            node.Accept(new NestedUseVisitor());
        }
    }
}