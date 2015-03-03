namespace Nicodem.Lexer
{
	public class RegExUnion : RegEx
	{
		public RegEx[] Regexes { private set; get; }

		internal RegExUnion ( params RegEx[] regexes )
		{
			this.Regexes = regexes;
		}

		public override int CompareTo (RegEx other)
		{
			//TODO(?)
			throw new System.NotImplementedException ();
		}
	}
}

