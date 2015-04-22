namespace Nicodem.Backend.Representation
{
    public class ConstantNode<TConstant> : Node
    {
        public ConstantNode(TConstant value)
        {
            Value = value;
        }

        // Value representation depends on architecture (and kind). Adjust when Target will be in shape.
        public TConstant Value { get; private set; }
    }
}