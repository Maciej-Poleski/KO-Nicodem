using System;
using System.Diagnostics;
using System.Linq;
using Nicodem.Semantics.CST;
using Nicodem.Source;

namespace Nicodem.Compiler
{
    /// <summary>
    ///     This is NIcodem Compiler driver
    /// </summary>
    internal static class Driver
    {
        private static void Main()
        {
            var args = Environment.GetCommandLineArgs();
            if (args.Length != 2)
            {
                Console.WriteLine("NIcodem Compiler driver\n");
                Console.WriteLine("{0} <input file>", args[0]);
                return;
            }
            var inputFile = new FileOrigin(args[1]);
            try
            {
                var tokenized = CSTBuilder.SanitizedTokens(inputFile);
                foreach (var tuple in tokenized)
                {
                    Debug.Assert(tuple.Item2.Count() == 1);
                    Console.Write("\"" + tuple.Item1.GetOriginText() + "\"(" + tuple.Item2.First() + ") ");
                }
            }
            catch (CSTBuilder.LexerFailure ex)
            {
                Console.WriteLine("Syntax error:");
                var diagnostics=new SourceDiagnostic();
                diagnostics.PrintFragmentInLine(ex.Fragment);
            }
        }
    }
}