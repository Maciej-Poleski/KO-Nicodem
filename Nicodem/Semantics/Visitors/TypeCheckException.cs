using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nicodem.Semantics.Visitors
{
    class TypeCheckException : Exception
    {
        public TypeCheckException() { }
        public TypeCheckException(string message) : base(message) { }
    }
}
