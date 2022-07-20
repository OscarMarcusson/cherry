using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Cherry.Internal.Reader;

namespace Cherry.Intermediate.Elements
{
	public class LinkElement : Element
	{
		public string DisplayName { get; set; }


		public LinkElement(VariablesCache parentVariables, LineReader reader, Element parent, CompilerArguments compilerArguments) : base(parentVariables, reader, parent, false, compilerArguments)
		{
		}


		protected override void OnLoad()
		{
			Name = "a";
			Type = ElementType.Link;

			DisplayName = Value;

			if (Configurations != null && Configurations.TryGetValue("name", out var displayName))
			{
				DisplayName = displayName;
				Configurations.Remove("name");
			}
		}


		protected override string HtmlTag => "a";

		protected override void WriteContentToHtml(StreamWriter writer, Document document, int indent, List<Element> children) => writer.Write(DisplayName);

		protected override void WriteCoreHtmlDefinition(StreamWriter writer)
		{
			base.WriteCoreHtmlDefinition(writer);
			writer.Write($" href=\"{Value}\"");
		}
	}
}
