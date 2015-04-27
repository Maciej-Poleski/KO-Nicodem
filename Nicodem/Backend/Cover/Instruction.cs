using System;
using System.Collections.Generic;

namespace Nicodem.Backend
{
	public class Instruction
	{
		// TODO
		public object RegisterUsed { get { throw new NotImplementedException (); } }

		// TODO
		public object RegisterDefined { get { throw new NotImplementedException (); } }

		// TODO
		public bool IsCopyInstruction { get { throw new NotImplementedException (); } }

		// TODO
		public string ToString(IReadOnlyDictionary<object, object> registerMapping) {
			throw new NotImplementedException ();
		}
	}
}

