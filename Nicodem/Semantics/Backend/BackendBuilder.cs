using System;
using System.Linq;
using System.Collections.Generic;
using B = Nicodem.Backend;
using Brep = Nicodem.Backend.Representation;
using Nicodem.Semantics.ExpressionGraph;

namespace Nicodem.Semantics
{
	class BackendBuilder
	{

		public static Brep.Node BuildBackend(IEnumerable<Vertex> expGraph, B.Function function)
		{
			return (new DFSBuilder(function)).Build(expGraph.First());
		}


		// --- private classes --------------------------------------

		private class DFSBuilder
		{
			private B.Function function;
			private Dictionary<Vertex, bool> visitted = new Dictionary<Vertex, bool>();
			private Dictionary<Vertex, Brep.Node> built = new Dictionary<Vertex, Brep.Node>();
			private Dictionary<Vertex, List<Action<Brep.Node>>> awaiting = new Dictionary<Vertex, List<Action<Brep.Node>>>();

			public DFSBuilder(B.Function function)
			{
				this.function = function;
			}

			public Brep.Node Build(Vertex expVertex)
			{
				visitted[expVertex] = true;
				Brep.Node newNode = null;

				if(expVertex is ConditionalJumpVertex) {

					var condVertex = expVertex as ConditionalJumpVertex;

					var condNode = NodeBuilder.BuildNode(condVertex.Expression, function);

					Action<Brep.Node> trueAct = null;
					Action<Brep.Node> falseAct = null;

					newNode = new Brep.ConditionalJumpNode(condNode, out trueAct, out falseAct);
					HandleAction(trueAct, condVertex.TrueJump);
					HandleAction(falseAct, condVertex.FalseJump);
				} else if(expVertex is OneJumpVertex) {

					// I interpret it as a sequence of two actions, therefore:
					var jumpVertex = expVertex as OneJumpVertex;

					var first = NodeBuilder.BuildNode(jumpVertex.Expression, function);

					Action<Brep.Node> secondAct = null;
					newNode = new Brep.SequenceNode(new List<Brep.Node>{first}, out secondAct);
					HandleAction(secondAct, jumpVertex.Jump);
				} else {

					var retVertex = expVertex as ReturnVertex;
					newNode = NodeBuilder.BuildNode(retVertex.Expression, function);
				}

				// finish build of dependent nodes
				if(awaiting.ContainsKey(expVertex)) {
					foreach(var act in awaiting[expVertex]) {
						act(newNode);
					}
				}

				built[expVertex] = newNode;

				return newNode;
			}

			private bool Buildable(Vertex expVertex) {
				return built.ContainsKey(expVertex) || !visitted.ContainsKey(expVertex);
			}

			private Brep.Node BuildNew(Vertex expVertex)
			{
				return built.ContainsKey(expVertex) ? built[expVertex] : Build(expVertex);
			}

			private void HandleAction(Action<Brep.Node> act, Vertex required)
			{
				if(Buildable(required)) {
					var node = BuildNew(required);
					act(node);
				} else {
					AppendAwaiting(required, act);
				}
			}

			private void AppendAwaiting(Vertex expVertex, Action<Brep.Node> act)
			{
				if(!awaiting.ContainsKey(expVertex)) {
					awaiting[expVertex] = new List<Action<Brep.Node>>();
				}
				awaiting[expVertex].Add(act);
			}
		}
	}
}

