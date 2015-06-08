using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using Nicodem.Semantics.CST;
using Nicodem.Semantics;
using Nicodem.Source;
using Nicodem.Backend;

namespace Nicodem.Compiler
{
    /// <summary>
    ///     This is NIcodem Compiler driver
    /// </summary>
    internal static class Driver
    {
		private static void generateAssemblyFile(string fileName, IEnumerable<string> assembly)
		{
			var stream = File.Open(fileName,FileMode.Create);
			using(var writer = new StreamWriter(stream))
			{
				foreach(var line in assembly)
				{
					writer.WriteLine(line);
				}
			}
		}

		private static void printHelp(string[] args)
		{
			Console.WriteLine("NIcodem Compiler driver\n");
			Console.WriteLine("{0} [options] <input file>", args[0]);
			Console.WriteLine();
			Console.WriteLine(" -s\tcompile only (generate assembly)");
			Console.WriteLine(" -o <name>\toutput file name");
		}

		private static void unknownParameter(string param)
		{
			Console.WriteLine("Unknown parameter: "+param);
		}

		private class Parameters
		{
			internal string outputFile;
			internal string inputFile;
			internal bool onlyAssembly;	// In such case generated output file should get ".s"

			public static Parameters parse(string[] args)
			{
				string outputFile=null;
				string inputFile=null;
				bool onlyAssembly=false;	// In such case generated output file should get ".s"
				for(int i=1;i<args.Length;++i)
				{
					string option=args[i];
					if(option.StartsWith("-"))
					{
						if(option.StartsWith("-s"))
						{
							if(option.Length>2)
							{
								unknownParameter(option);
								return null;
							}
							onlyAssembly=true;
						}
						else if(option.StartsWith("-o"))
						{
							if(option.Length>2)
							{
								outputFile=option.Substring(2);
							}
							else
							{
								outputFile=args[i+1];
								i++;
							}
						}
						else
						{
							unknownParameter(option);
							return null;
						}
					}
					else
					{
						if(inputFile==null)
						{
							inputFile=option;
						}
						else
						{
							unknownParameter(option);
							return null;
						}
					}
				}
				if(inputFile==null)
				{
					Console.WriteLine("Input file name is required");
					return null;
				}
				if(outputFile==null)
				{
					outputFile=inputFile+ (onlyAssembly? ".asm" : ".o");
				}
				return new Parameters() { outputFile = outputFile, inputFile = inputFile, onlyAssembly = onlyAssembly};
			}
		}

		// System.IO.Path.GetTempFileName()

        private static void Main()
        {
            var args = Environment.GetCommandLineArgs();
			var parameters=Parameters.parse(args);
			if(parameters==null)
			{
				return;
			}
			if(parameters.outputFile==null)
			{
				parameters.outputFile = System.IO.Path.GetTempFileName();
			}

            var inputFile = new FileOrigin(args[1]);
            try
            {
                var parseTree = CSTBuilder.Build(inputFile);
                Console.WriteLine(parseTree.ToString());

				var backendFunctions = new Frontend().FromParseTreeToBackend(parseTree);

				var backend=new Backend.Backend();
				string assemblyFile=parameters.outputFile;
				if(!parameters.onlyAssembly)
				{
					assemblyFile=System.IO.Path.GetTempFileName();
				}
				generateAssemblyFile(assemblyFile, backend.FromFunctionsToNasm(backendFunctions));
				if(!parameters.onlyAssembly)
				{
					// TODO invoke NASM (using parameters)
					var nasm=new Process();
					nasm.StartInfo.FileName="nasm";	// take $PATH into account
					nasm.StartInfo.Arguments=assemblyFile+" -o "+parameters.outputFile;
					nasm.Start();
					nasm.WaitForExit();
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