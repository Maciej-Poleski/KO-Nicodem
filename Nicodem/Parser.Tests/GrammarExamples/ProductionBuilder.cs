using System;
using System.Text;
using System.Collections.Generic;

namespace Nicodem.Parser.Tests
{
	internal class ProductionBuilder
	{
		private Dictionary<string, char> _map = new Dictionary<string, char>();
		private char lastTerminal = 'a';
		private char lastNonTerminal = 'A';

		public StringProduction GetProduction(string from, string[] to)
		{
			if(!_map.ContainsKey(from)) {
				AddNonTerminal(from);
			}

			StringBuilder builder = new StringBuilder();
			foreach(var str in to) {
				if(!_map.ContainsKey(str)) {
					if(char.IsUpper(str[0])) {
						AddNonTerminal(str);
					} else if(str[0] == '$') {
						_map[str] = '$';
					} else {
						AddTerminal(str);
					}
				}
				builder.Append(_map[str]);
			}
			return new StringProduction(_map[from], builder.ToString());
		}

		private void AddNonTerminal(string term)
		{
			_map[term] = lastNonTerminal;
			lastNonTerminal++;
		}

		private void AddTerminal(string term)
		{
			_map[term] = lastTerminal;
			lastTerminal++;
		}

		public CharSymbol GetSymbolForTerm(string from)
		{
			return new CharSymbol(_map[from]);
		}
	}
}

