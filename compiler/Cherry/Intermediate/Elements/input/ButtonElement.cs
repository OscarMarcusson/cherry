using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Cherry.Internal.Reader;

namespace Cherry.Intermediate.Elements
{
	public class ButtonElement : InputElementBase
	{
		public ButtonElement(VariablesCache parentVariables, LineReader reader, Element parent, CompilerArguments compilerArguments) : base(parentVariables, reader, parent, compilerArguments)
		{
		}


		protected override ElementType GetType => ElementType.Button;
		protected override string GetTypeString => "button";
	}
}
