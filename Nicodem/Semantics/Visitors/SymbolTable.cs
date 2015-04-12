using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Nicodem.Semantics.AST;

namespace Nicodem.Semantics
{
	class SymbolTable
	{
		private Dictionary<String, Stack<VariableDefNode> > symbolTable = new Dictionary<string, Stack<VariableDefNode>>();
		private Stack<int> blocks = new Stack<int>();
		private Stack<String> variables = new Stack<String> ();
		private int variableCounter = 0;

		public SymbolTable ()
		{
		}

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

		public void Insert (String name, VariableDefNode variable) 
		{
			variableCounter++;
			variables.Push (name);

			if (!symbolTable.ContainsKey (name))
				symbolTable [name] = new Stack<VariableDefNode> ();

			symbolTable [name].Push (variable);
		}

		public VariableDefNode LookUp (String name)
		{
			if( !symbolTable.ContainsKey(name) )
				throw new DefinitionNotFoundException();

			return symbolTable [name].Peek ();
		}
	}
}

