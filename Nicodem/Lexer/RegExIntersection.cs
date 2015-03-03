namespace Nicodem.Lexer
{
	public class RegExIntersection : RegEx
	{
		public RegEx[] Regexes { private set; get; }

		internal RegExIntersection ( params RegEx[] regexes )
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

