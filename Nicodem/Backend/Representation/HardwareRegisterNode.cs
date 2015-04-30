namespace Nicodem.Backend.Representation
{
    public class HardwareRegisterNode : RegisterNode
    {
        public HardwareRegisterNode(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }

        public override string ToString()
        {
            return string.Format("Hardware Register <{0}>", Name);
        }
    }
}