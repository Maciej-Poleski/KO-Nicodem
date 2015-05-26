using System;
using System.Linq;
using System.Collections.Generic;
using Nicodem.Semantics.AST;
using Nicodem.Backend;

namespace Nicodem.Semantics.Visitors
{
    internal class FunctionSplitterVisitor : AbstractRecursiveVisitor
    {
        private readonly List<FunctionDefinitionNode> _functions = new List<FunctionDefinitionNode>();

        internal IReadOnlyList<FunctionDefinitionNode> Functions
        {
            get { return _functions; }
        }

        public override void Visit(FunctionDefinitionNode node)
        {
            _functions.Add(node);
            base.Visit(node);
        }
    }

    internal class FunctionSplitter2Visitor : AbstractRecursiveVisitor
    {
        private readonly List<Function> _functions = new List<Function>();
        private Function _currentFunction;

        internal IReadOnlyList<Function> Functions
        {
            get { return _functions; }
        }

        public override void Visit(FunctionDefinitionNode node)
        {
            var oldFunction = _currentFunction;
            var parametersBitmap = node.Parameters.Select(p => p.NestedUse).ToArray();
            _currentFunction = new Function(node.Name, parametersBitmap, oldFunction);  // no name mangling for now - no function name overloading
            node.BackendFunction = _currentFunction;
            _functions.Add(_currentFunction);
            base.Visit(node);
            _currentFunction = oldFunction;
        }
    }

    public static partial class Extensions
    {
        /// <summary>
        /// Return all function definitions contained in this program AST.
        /// </summary>
        internal static IReadOnlyCollection<FunctionDefinitionNode> GetAllFunctionDefinitions(
            this ProgramNode node)
        {
            var visitor = new FunctionSplitterVisitor();
            node.Accept(visitor);
            return visitor.Functions;
        }
    }
}