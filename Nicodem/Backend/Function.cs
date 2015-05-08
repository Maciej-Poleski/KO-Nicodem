using System;
using Nicodem.Backend.Representation;

namespace Nicodem.Backend
{
	public class Function
	{
		public Function ()
		{
		}

		public Local AllocLocal()
		{
			return new Local();
		}

		public Node AccessLocal(Local local)
		{
			throw new NotImplementedException();
		}

		public SequenceNode FunctionCall(Temporary[] args, Temporary result)
		{
			throw new NotImplementedException();
		}

	}
}

