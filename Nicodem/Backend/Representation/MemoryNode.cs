namespace Nicodem.Backend.Representation
{
    public class MemoryNode : LocationNode
    {
        public MemoryNode(Node address)
        {
            Address = address;
        }

        
        public Node Address { get; private set; }

		public override Node[] GetChildren() {
			return new Node[]{ Address };
		}

        #region Printing
        protected override string PrintElements(string prefix)
        {
            return PrintChild(prefix, "address", Address);
        }
        #endregion

        #region implemented ReplaceRegisterWithLocal
        internal override Node ReplaceRegisterWithLocal(
            System.Collections.Generic.IReadOnlyDictionary<RegisterNode, Local> map, 
            System.Collections.Generic.List<Node> newTrees, 
            Function f)
        {
            Address = Address.ReplaceRegisterWithLocal(map, newTrees, f);
            return base.ReplaceRegisterWithLocal(map, newTrees, f);
        }
        #endregion
    }
}