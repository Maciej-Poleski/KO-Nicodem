namespace Nicodem.Backend.Representation
{
	public class ConditionalJumpToLabelNode : Node
	{
		public Node Condition { get; private set; }

		public LabelNode NextNode { get; private set; }

		public ConditionalJumpToLabelNode (Node condition, LabelNode nextNode)
		{
			Condition = condition;
			NextNode = nextNode;
		}

		public override Node[] GetChildren() {
			return new Node[]{ Condition, NextNode };
		}

        #region Printing
        protected override string PrintElements(string prefix)
        {
            return PrintChild(prefix, "condition", Condition) + PrintChild(prefix, "jump_to", NextNode);
        }
        #endregion

        #region implemented ReplaceRegisterWithLocal
        internal override Node ReplaceRegisterWithLocal(
            System.Collections.Generic.IReadOnlyDictionary<RegisterNode, Local> map, 
            System.Collections.Generic.List<Node> newTrees, 
            Function f)
        {
            Condition = Condition.ReplaceRegisterWithLocal(map, newTrees, f);
            return base.ReplaceRegisterWithLocal(map, newTrees, f);
        }
        #endregion
	}
}
