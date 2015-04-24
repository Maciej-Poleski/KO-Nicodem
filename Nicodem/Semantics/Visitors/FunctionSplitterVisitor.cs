using System.Collections.Generic;
using Nicodem.Semantics.AST;

namespace Nicodem.Semantics.Visitors
{
    internal class FunctionSplitterVisitor : AbstractRecursiveVisitor
    {
        private readonly List<FunctionDefinitionExpression> _functions = new List<FunctionDefinitionExpression>();

        internal IReadOnlyList<FunctionDefinitionExpression> Functions
        {
            get { return _functions; }
        }

        public override void Visit(FunctionDefinitionExpression node)
        {
            _functions.Add(node);
            base.Visit(node);
        }
    }

    public static partial class Extensions
    {
        internal static IReadOnlyCollection<FunctionDefinitionExpression> GetAllFunctionDefinitions(
            this ProgramNode node)
        {
            var visitor = new FunctionSplitterVisitor();
            node.Accept(visitor);
            return visitor.Functions;
        }
    }
}