using System;
using System.Collections.Generic;
using System.Diagnostics;
using Nicodem.Source;

namespace Nicodem.Lexer
{
    public class Lexer
    {
        public Lexer(RegEx[] regexCategories)
        {
        }

        /// <summary>
        /// Może klient mógłby określić jaki chce typ enumeratora...
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        IEnumerable<Tuple<Fragment, IEnumerable<int>>> Process(IOrigin source) { } 
    }
}