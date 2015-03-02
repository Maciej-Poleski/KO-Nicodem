namespace Nicodem.Lexer
{
	internal class RegExConcat : RegEx
	{
		private readonly RegEx[] regexes;

		internal RegExConcat ( params RegEx[] regexes )
		{
			this.regexes = regexes;
		}

		public override int CompareTo (RegEx other)
		{
			//TODO(pmikos)
			throw new System.NotImplementedException ();
		}
	}
}

