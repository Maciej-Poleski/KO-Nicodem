using System;
using Nicodem.Semantics.AST;
using Nicodem.Backend;
using Brep = Nicodem.Backend.Representation;
using System.Collections.Generic;

namespace Nicodem.Semantics.Visitors
{
	internal class FunctionLocalVisitor : AbstractRecursiveVisitor
	{
		private readonly Target target;
		private Stack<Function> functions = new Stack<Function> ();

		internal FunctionLocalVisitor(Target target) {
			this.target = target;
		}

		public override void Visit(FunctionDefinitionNode node)
		{
			node.BackendFunction = target.CreateFunction ();
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

