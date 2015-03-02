namespace Nicodem.Lexer
{
	internal class RegExUnion : RegEx
	{
		internal RegEx[] Regexes { private set; get; }

		internal RegExUnion ( params RegEx[] regexes )
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

