namespace Nicodem.Lexer
{
	internal class RegExComplement : RegEx
	{
		internal RegEx Regex { private set; get; }

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

