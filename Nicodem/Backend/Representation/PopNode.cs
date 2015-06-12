namespace Nicodem.Backend.Representation
{
	public class PopNode : Node
	{
		public Node Value { get; private set; }

		public PopNode( Node val ) {
			Value = val;
		}

		public override Node[] GetChildren() {
			return new[]{ Value };
		}

        #region Printing
        protected override string PrintElements(string prefix)
        {
            return PrintChild(prefix, "value", Value);
        }
        #endregion

        #region implemented ReplaceRegisterWithLocal
        internal override Node ReplaceRegisterWithLocal(
            System.Collections.Generic.IReadOnlyDictionary<RegisterNode, Local> map, 
            System.Collections.Generic.List<Node> newTrees, 
            Function f)
        {
            Value = Value.ReplaceRegisterWithLocal(map, newTrees, f);
            return base.ReplaceRegisterWithLocal(map, newTrees, f);
        }
        #endregion
	}
}

