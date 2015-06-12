namespace Nicodem.Backend.Representation
{
	public class HardwareRegisterNode : RegisterNode
	{
		public HardwareRegisterNode (string name)
		{
			Name = name;
		}

		public string Name { get; private set; }

        public override string Id { get {return Name;} }
	}
}