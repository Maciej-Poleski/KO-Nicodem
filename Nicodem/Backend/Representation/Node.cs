using System;
using System.Collections.Generic;
using Nicodem.Backend.Cover;

namespace Nicodem.Backend.Representation
{
    public abstract class Node : IValuable
    {
        protected TemporaryNode _value;

		// optimumCovering consists of costs, tile covering the root and list of children under the tile.
		Tuple<int, Tile, List<Node> > optimumCovering;

        protected Node()
        {
            Accept = NodeMixin.MakeAcceptThunk(this);
        }

        public Action<AbstractVisitor> Accept { get; private set; }

        // I can move it down inheritance hierarchy
        public virtual RegisterNode Value
        {
            get { return _value ?? (_value = new TemporaryNode()); }
        }

		public virtual Node[] GetChildren () {
			return new Node[]{};
		}

		// Compare checks whether the tile given as an argument can cover this Node. If yes, it returns tuple
		// of the form (true, list of children under the tile). If not, it returns instead (false, null).
		Tuple<bool, List<Node>> Compare(Tile tile) {
			// exceptional case for assign { mem {reg}, reg }
			if (this.GetType () == typeof(AssignmentNode) && (this.GetChildren () [0]).GetType () == typeof(MemoryNode)) {
				if(tile.Type == typeof(AssignmentNode) && (tile.Children[0].Type) != typeof(MemoryNode)) {
					return new Tuple<bool, List<Node>> (false, null);
				}
			}
			if (tile.Type == typeof(RegisterNode)) {
				if (this.GetType () == typeof(RegisterNode)) {
					return new Tuple<bool, List<Node>> (true, new List<Node> ());
				} else {
					return new Tuple<bool, List<Node>> (true, new List<Node>{ this });
				}
			}
			if (tile.Type != this.GetType ())
				return new Tuple<bool, List<Node>> (false, null);
			var resultChildren = new List<Node> ();

			for (int i = 0; i < this.GetChildren ().Length; i++) {
				var res = this.GetChildren () [i].Compare (tile.Children [i]);
				if (res.Item1 == false)
					return new Tuple<bool, List<Node>> (false, null);
				foreach (var child in res.Item2) {
					resultChildren.Add (child);
				}
			}
			return new Tuple<bool, List<Node>> (true, resultChildren);
		}

		// Computes optimum covering of this node with tiles given as an argument.
		void ComputeOptimumCovering(IEnumerable<Tile> tiles) {
			// recursively compute optimum covering for children
			foreach (var child in this.GetChildren()) {
				child.ComputeOptimumCovering (tiles);
			}
			this.optimumCovering = new Tuple<int, Tile, List<Node>> (int.MaxValue, null, null);
			// check each tile and choose the one that minimizes the cost.
			foreach (var tile in tiles) {
				var compareResult = this.Compare (tile); // returns Tuple{true/false, list of children}
				// if the node can be covered by this tile
				if (compareResult.Item1 == true) {
					int coveringCost = tile.Cost;
					// for each child under the tile add its optimum covering cost to the currently computed coveringCost
					foreach (var child in compareResult.Item2)
						coveringCost += child.optimumCovering.Item1;

					if (coveringCost < this.optimumCovering.Item1)
						this.optimumCovering = new Tuple<int, Tile, List<Node>> (coveringCost, tile, compareResult.Item2);
				}
			}
		}
    }
}