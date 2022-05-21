using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SimplifiedUserInterfaceFramework.Internal.Reader;

namespace SimplifiedUserInterfaceFramework.Intermediate.Elements
{
	public class TextboxElement : InputElementBase
	{
		public TextboxElement(LineReader reader, Element parent, CompilerArguments compilerArguments) : base(reader, parent, compilerArguments)
		{
		}

		protected override string GetTypeString => "text";
		protected override ElementType GetType => ElementType.Textbox;
	}
}
