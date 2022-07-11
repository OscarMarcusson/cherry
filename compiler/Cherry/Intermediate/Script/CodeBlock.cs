using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimplifiedUserInterfaceFramework.Intermediate
{
	public class CodeBlock
	{
		readonly Dictionary<string, Function> Functions = new Dictionary<string, Function>();
		readonly Dictionary<string, Variable> Variables = new Dictionary<string, Variable>();

		public bool HasContent => Functions.Count > 0 || Variables.Count > 0;

		public Variable[] GetVariables() => Variables.Select(x => x.Value).ToArray();
		public Function[] GetFunctions() => Functions.Select(x => x.Value).ToArray();



		public bool FunctionExists(string name) => Functions.ContainsKey(name);
		public bool VariableExists(string name) => Variables.ContainsKey(name);


		public void Add(Variable variable) => Variables[variable.Name] = variable;
		public void Add(Function function) => Functions[function.Name] = function;
	}
}
