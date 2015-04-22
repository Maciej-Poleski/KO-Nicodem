namespace Nicodem.Backend.Representation
{
    public class AssignmentNode : Node
    {
        public AssignmentNode(LocationNode target, LocationNode source)
        {
            Target = target;
            Source = source;
        }

        public LocationNode Target { get; private set; }
        public LocationNode Source { get; private set; }
    }
}