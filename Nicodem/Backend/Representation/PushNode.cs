namespace Nicodem.Backend.Representation
{
	public class PushNode : Node
	{
		public Node Value { get; private set; }

		public PushNode( Node val ) {
			Value = val;
		}

		public override Node[] GetChildren() {
			return new[]{ Value };
		}

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

