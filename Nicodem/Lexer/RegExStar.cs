namespace Nicodem.Lexer
{
	internal class RegExStar : RegEx
	{
		internal RegEx Regex { private set; get; }

		internal RegExStar ( RegEx regex )
		{
			this.Regex = regex;
		}

		public override int CompareTo (RegEx other)
		{
			//TODO(pmikos)
			throw new System.NotImplementedException ();
		}
	}
}

