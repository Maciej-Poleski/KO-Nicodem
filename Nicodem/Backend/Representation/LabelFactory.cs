using System;
using Nicodem.Backend.Representation;

namespace Nicodem.Backend.Representation
{
	public static class LabelFactory
	{
		private static int counter = 0;

		public static LabelNode NextLabel ()
		{
			++counter;
			return new LabelNode ("label" + counter);
		}
	}
}
