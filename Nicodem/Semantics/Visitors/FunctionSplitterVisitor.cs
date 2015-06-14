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