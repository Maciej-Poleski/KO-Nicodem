using System;
using Nicodem.Semantics.AST;
using Nicodem.Backend;
using System.Collections.Generic;

namespace Nicodem.Semantics.Visitors
{
	internal class FunctionLocalVisitor : AbstractRecursiveVisitor
	{
		private readonly Target target;
		private Function function;

		internal FunctionLocalVisitor(Target target) {
			this.target = target;
		}

		public override void Visit(FunctionDefinitionExpression node)
		{
			node.BackendFunction = target.CreateFunction ();
			function = node.BackendFunction;

			base.Visit(node);
		}

		public override void Visit(VariableDefNode node)
		{
			if (node.NestedUse) {
				function.AllocLocal ();
			} else {
				node.VariableLocation = new Temporary ();
			}

			base.Visit(node);
		}
	}
}

