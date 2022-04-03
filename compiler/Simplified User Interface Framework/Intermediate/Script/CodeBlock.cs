using System;
using System.Collections.Generic;
using System.Text;

namespace SimplifiedUserInterfaceFramework.Intermediate
{
	public class CodeBlock
	{
		readonly Dictionary<string, Function> Functions = new Dictionary<string, Function>();
		readonly Dictionary<string, Variable> Variables = new Dictionary<string, Variable>();






		public bool FunctionExists(string name) => Functions.ContainsKey(name);
		public bool VariableExists(string name) => Variables.ContainsKey(name);


		public void Add(Variable variable) => Variables[variable.Name] = variable;
	}
}
