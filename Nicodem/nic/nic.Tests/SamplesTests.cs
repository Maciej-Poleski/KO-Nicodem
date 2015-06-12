using NUnit.Framework;
using System;
using System.Text;
using Nicodem.Semantics.CST;
using Nicodem.Semantics;
using Nicodem.Source;

namespace nic.Tests
{
    [TestFixture()]
    public class Test
    {
        //[Test()]
        public void Sample01Test() // bad test -> strings not implemented
        {
            var s = new StringBuilder();
            s.AppendLine("main(int x) -> void");
            s.AppendLine("{");
            s.AppendLine("    print(\"Hello, World!\\n\");");
            s.AppendLine("}");
            var inputFile = new StringOrigin(s.ToString());
            var parseTree = CSTBuilder.Build(inputFile);
            var backendFunctions = new Frontend().FromParseTreeToBackend(parseTree);

            foreach(var f in backendFunctions)
            {
                Console.WriteLine(f);
            }
        }
    }
}

