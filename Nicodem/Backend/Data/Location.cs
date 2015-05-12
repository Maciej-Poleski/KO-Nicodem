using System;
using Nicodem.Backend.Representation;

namespace Nicodem.Backend
{
    // This class is candidate for removal. Try not to use if You can.
    [Obsolete]
	public abstract class Location
	{
	    public abstract LocationNode AccessLocal(Function function);
	}
}

