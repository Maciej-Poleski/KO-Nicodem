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
            _functions.Add(_currentFunction);
            base.Visit(node);
            _currentFunction = oldFunction;
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
        internal static IReadOnlyCollection<FunctionDefinitionNode> SplitIntoFunctions(this ProgramNode node)
        {
            var visitor = new FunctionSplitterVisitor();
            node.Accept(visitor);
            return visitor.Functions.Select(f => new FunctionDefinitionNode() { 
                Name = f.Name, 
                Parameters = f.Parameters, 
                BackendFunction = f.BackendFunction, 
                Body = filterBody(f.Body), 
                ExpressionType = f.ExpressionType, 
                Fragment = f.Fragment,
                ResultType = f.ResultType
            }).ToArray();
        }

        private static ExpressionNode filterBody(ExpressionNode expressionNode)
        {
            return expressionNode;  // TODO filter out nested functions
        }

        internal static IReadOnlyCollection<Function> SplitIntoBackendFunctions(this ProgramNode node)
        {
            var visitor = new FunctionSplitter2Visitor();
            node.Accept(visitor);
            return visitor.Functions;
        }
    }
}