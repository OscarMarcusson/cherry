using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SimplifiedUserInterfaceFramework.Internal.Reader;

namespace SimplifiedUserInterfaceFramework.Intermediate.Elements
{
	public class TextboxElement : InputElementBase
	{
		public TextboxElement(VariablesCache parentVariables, LineReader reader, Element parent, CompilerArguments compilerArguments) : base(parentVariables, reader, parent, compilerArguments)
		{
		}

		protected override string GetTypeString => "text";
		protected override ElementType GetType => ElementType.Textbox;
	}
}
