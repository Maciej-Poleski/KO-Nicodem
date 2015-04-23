using System;
using System.Collections.Generic;
using Nicodem.Semantics.AST;

namespace Nicodem.Semantics.Visitors
{
    /// <summary>
    ///     Names of all variables must be resolved before use of this visitor.
    /// </summary>
    internal class NestedUseVisitor : AbstractRecursiveVisitor
    {
        private readonly Dictionary<VariableDeclNode, FunctionNode> _declToFunction =
            new Dictionary<VariableDeclNode, FunctionNode>();

        private FunctionNode _currentFunction;

        public override void Visit(FunctionNode node)
        {
            _currentFunction = node;
            base.Visit(node);
        }

        // TODO implement two pass algorithm (need 2 visitors one-by-one)
        public override void Visit(VariableUseNode node)
        {
            Visit(node as ExpressionNode);
            if (!_declToFunction.ContainsKey(node.Declaration))
            {
                throw new NotImplementedException(node +
                                                  " is unknown. Implement two-pass visitor to handle this source code.");
            }
            if (_declToFunction[node.Declaration] != _currentFunction)
            {
                node.Declaration.NestedUse = true;
            }
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
}