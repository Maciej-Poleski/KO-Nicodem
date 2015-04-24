using System;
using System.Collections.Generic;
using Nicodem.Semantics.AST;

namespace Nicodem.Semantics.Visitors
{
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
        private readonly Dictionary<VariableDeclNode, FunctionNode> _declToFunction =
            new Dictionary<VariableDeclNode, FunctionNode>();

        private FunctionNode _currentFunction;

        internal IReadOnlyDictionary<VariableDeclNode, FunctionNode> DeclToFunction
        {
            get { return _declToFunction; }
        }

        public override void Visit(FunctionNode node)
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
        private readonly IReadOnlyDictionary<VariableDeclNode, FunctionNode> _declToFunction;
        private FunctionNode _currentFunction;

        public NestedUseVisitor2(IReadOnlyDictionary<VariableDeclNode, FunctionNode> declToFunction)
        {
            _declToFunction = declToFunction;
        }

        public override void Visit(FunctionNode node)
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
        internal static void FillInNestedUseFlag(this ProgramNode node)
        {
            node.Accept(new NestedUseVisitor());
        }
    }
}