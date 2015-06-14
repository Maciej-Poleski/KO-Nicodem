namespace Nicodem.Backend.Representation
{
    public class AssignmentNode : Node
    {
        public AssignmentNode(LocationNode target, Node source)
        {
            Target = target;
            Source = source;
        }

        public LocationNode Target { get; private set; }
        public Node Source { get; private set; }

		public override Node[] GetChildren() {
			return new Node[]{ Target, Source };
		}

        #region Printing
        protected override string PrintElements(string prefix)
        {
            return PrintChild(prefix, "target", Target) + PrintChild(prefix, "source", Source);
        }
        #endregion

        #region implemented ReplaceRegisterWithLocal
        internal override Node ReplaceRegisterWithLocal(
            System.Collections.Generic.IReadOnlyDictionary<RegisterNode, Local> map, 
            System.Collections.Generic.List<Node> newTrees, 
            Function f)
        {
            Target = (LocationNode) Target.ReplaceRegisterWithLocal(map, newTrees, f);
            Source = Source.ReplaceRegisterWithLocal(map, newTrees, f);
            return base.ReplaceRegisterWithLocal(map, newTrees, f);
        }
        #endregion

    }
}