namespace Nicodem.Lexer
{
	public class RegExRange : RegEx
	{
		public char Character { private set; get; }

		internal RegExRange ( char c )
		{
			this.Character = c;
		}

		public override int CompareTo (RegEx other)
		{
			//TODO(pmikos)
			throw new System.NotImplementedException ();
		}
	}
}

