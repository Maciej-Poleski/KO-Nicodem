using System;

namespace Nicodem.Lexer
{
	public static class RegExFactory
	{
		public static RegEx Empty() 
		{
			// TODO(pmikos)
			throw new NotImplementedException ();
		}

		public static RegEx Range( char c )
		{
			// TODO(pmikos)
			throw new NotImplementedException ();
		}

		public static RegEx Union( params RegEx[] regexes )
		{
			// TODO(pmikos)
			throw new NotImplementedException ();
		}

		public static RegEx Intersection( params RegEx[] regexes )
		{
			// TODO(pmikos)
			throw new NotImplementedException ();
		}

		public static RegEx Concat( params RegEx[] regexes )
		{
			// TODO(pmikos)
			throw new NotImplementedException ();
		}

		public static RegEx Star( RegEx regex )
		{
			// TODO(pmikos)
			throw new NotImplementedException ();
		}

		public static RegEx Complement( RegEx regex )
		{
			// TODO(pmikos)
			throw new NotImplementedException ();
		}
	}
}

