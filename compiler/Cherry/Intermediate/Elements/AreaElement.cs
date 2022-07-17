using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Cherry.Internal.Reader;

namespace Cherry.Intermediate.Elements
{
	public class AreaElement : Element
	{
		public AreaElement(VariablesCache parentVariables, LineReader reader, Element parent, CompilerArguments compilerArguments) : base(parentVariables, reader, parent, false, compilerArguments)
		{
		}


		protected override void OnLoad()
		{
			Name = "area";
			Type = ElementType.None;
		}



		protected override void WriteCoreHtmlDefinition(StreamWriter writer)
		{
			writer.Write($"div{HtmlFormattedClasses()}");
		}

		protected override void WriteContentToHtml(StreamWriter writer, Document document, int indent, List<Element> children) => base.WriteContentToHtml(writer, document, indent, children);
	}
}
