using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SimplifiedUserInterfaceFramework.Intermediate
{
	public abstract class CodeLine
	{
		public readonly VariablesCache Variables;

		public CodeLine(VariablesCache parentVariables) => Variables = new VariablesCache(parentVariables);

		public abstract void ToJavascriptStream(StreamWriter writer, int indentation = 0);
	}
}
