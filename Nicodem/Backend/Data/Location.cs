using System;
using Nicodem.Backend.Representation;

namespace Nicodem.Backend
{
	public abstract class Location
	{
	    public abstract LocationNode AccessLocal(Function function, LocationNode stackFrame);
	}
}

