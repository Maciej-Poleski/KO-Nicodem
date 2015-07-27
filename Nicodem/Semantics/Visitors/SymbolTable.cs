using System;
using System.Collections.Generic;

namespace Nicodem.Semantics
{
    class SymbolTable<T> 
	{
		readonly Dictionary<String, Stack<T> > symbolTable = new Dictionary<string, Stack<T>>();
		Stack<int> blocks = new Stack<int>();
		Stack<String> variables = new Stack<String> ();
		int variableCounter = 0;

		public void OpenBlock() 
		{
			blocks.Push (variableCounter);
			variableCounter = 0;
		}

		public void CloseBlock() 
		{
			for (int i = 0; i < variableCounter; i++) {
				String name = variables.Pop ();
				symbolTable [name].Pop ();
			}

			variableCounter = blocks.Pop ();
		}

        public void Insert (String name, T variable) 
		{
			variableCounter++;
			variables.Push (name);

			if (!symbolTable.ContainsKey (name))
                symbolTable [name] = new Stack<T> ();

			symbolTable [name].Push (variable);
		}

		public S LookUp<S> (String name) where S : class, T
		{
			if (!symbolTable.ContainsKey (name))
				throw new DefinitionNotFoundException ();

			var result = symbolTable [name].Peek () as S;
			if (result == null)
				throw new DefinitionNotFoundException ();

			return result;
		}
	}
}

