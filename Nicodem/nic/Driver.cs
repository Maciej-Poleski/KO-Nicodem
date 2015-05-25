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
                var parseTree = CSTBuilder.Build(inputFile);
                Console.WriteLine(parseTree.ToString());
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