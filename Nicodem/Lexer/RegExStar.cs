namespace Nicodem.Lexer
{
	public class RegExStar : RegEx
	{
		public RegEx Regex { private set; get; }

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

