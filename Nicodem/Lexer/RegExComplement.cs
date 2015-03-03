namespace Nicodem.Lexer
{
	public class RegExComplement : RegEx
	{
		public RegEx Regex { private set; get; }

		internal RegExComplement ( RegEx regex )
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

