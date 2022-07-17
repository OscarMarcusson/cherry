using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Cherry.Internal.Reader;

namespace Cherry.Intermediate.Elements
{
	public class TextElement : Element
	{
		public TextElement(VariablesCache parentVariables, LineReader reader, Element parent, CompilerArguments compilerArguments) : base(parentVariables, reader, parent, false, compilerArguments)
		{
		}


		protected override void OnLoad()
		{
			Name = "text";
			Type = ElementType.None;
		}



		protected override void WriteCoreHtmlDefinition(StreamWriter writer)
		{
			writer.Write($"p{HtmlFormattedClasses()}");
		}

		internal override void ToEndHtmlStream(StreamWriter writer, int customIndent = 0) => writer.WriteLine($"</p>");
	}
}
