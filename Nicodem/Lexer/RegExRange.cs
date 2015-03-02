namespace Nicodem.Lexer
{
	internal class RegExRange : RegEx
	{
		internal char Character { private set; get; }

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

