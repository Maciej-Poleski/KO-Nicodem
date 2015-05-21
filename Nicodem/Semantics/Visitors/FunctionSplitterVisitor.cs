using System;
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
        internal static IReadOnlyCollection<FunctionDefinitionNode> GetAllFunctionDefinitions(
            this ProgramNode node)
        {
            var visitor = new FunctionSplitterVisitor();
            node.Accept(visitor);
            return visitor.Functions;
        }

        /// <summary>
        /// Split program AST into functions. Each function cannot contain nested function code.
        /// </summary>
       internal static IReadOnlyCollection<FunctionDefinitionNode>  SplitIntoFunctions(this ProgramNode node)
        {
            // TODO: implement!
            throw new NotImplementedException("Please, implement me!");
        }
    }
}