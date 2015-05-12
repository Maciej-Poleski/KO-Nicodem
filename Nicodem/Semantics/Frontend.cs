using System;
using Nicodem.Parser;

namespace Nicodem.Semantics
{
    /// <summary>
    /// This is main front-end class. Use it to go through front-end phase.
    /// </summary>
    public class Frontend
    {
        public void FromParseTreeToBackend<TSymbol>(IParseTree<TSymbol> parseTree) where TSymbol:ISymbol<TSymbol>
        {
        }
    }
}
