namespace Nicodem.Lexer
{
	internal class RegExConcat : RegEx
	{
		internal RegEx[] Regexes { private set; get; }

		internal RegExConcat ( params RegEx[] regexes )
		{
			this.Regexes = regexes;
		}

		public override int CompareTo (RegEx other)
		{
			//TODO(pmikos)
			throw new System.NotImplementedException ();
		}
	}
}

