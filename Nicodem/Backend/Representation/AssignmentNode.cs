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
    }
}