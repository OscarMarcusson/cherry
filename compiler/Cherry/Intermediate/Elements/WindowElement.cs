using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Cherry.Internal.Reader;

namespace Cherry.Intermediate.Elements
{
	public class WindowElement : Element
	{
		public WindowElement(VariablesCache parentVariables, LineReader reader, Element parent, CompilerArguments compilerArguments) : base(parentVariables, reader, parent, false, compilerArguments)
		{
			if (parent != null)
				throw new SectionException(reader.Text, 0, reader.First.Length, "A window can only be used at the root level", reader.LineNumber);
		}


		protected override void OnLoad()
		{
			Name = "area";
			Type = ElementType.None;
		}


		protected override string HtmlTag => $"body";
	}
}
