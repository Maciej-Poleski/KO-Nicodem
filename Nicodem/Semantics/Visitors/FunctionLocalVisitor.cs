using System;
using System.Linq;
using Nicodem.Semantics.AST;
using Nicodem.Backend;
using Brep = Nicodem.Backend.Representation;
using System.Collections.Generic;

namespace Nicodem.Semantics.Visitors
{
	internal class FunctionLocalVisitor : AbstractRecursiveVisitor
	{
		private Stack<Function> functions = new Stack<Function> ();

		public override void Visit(FunctionDefinitionNode node)
		{
            // create this function backend object
            var parametersBitmap = node.Parameters.Select(p => p.NestedUse).ToArray();
            var enclosedIn = (functions.Count > 0) ? functions.Peek() : null; // peek od stack - parent function
            node.BackendFunction = new Function(node.Name, parametersBitmap, enclosedIn);  // no name mangling for now - no function name overloading
			// continue visiting, place this function on stack, remove it when le
            functions.Push (node.BackendFunction);
            base.Visit(node);
			functions.Pop ();
		}

		public override void Visit(VariableDefNode node)
		{
			if (node.NestedUse) {
				functions.Peek().AllocLocal ();
			} else {
				node.VariableLocation = new Temporary();
			}

			base.Visit(node);
		}
	}
}

