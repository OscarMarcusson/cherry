using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SimplifiedUserInterfaceFramework.Internal.Reader;

namespace SimplifiedUserInterfaceFramework.Intermediate.Elements
{
	public class TextboxElement : Element
	{
		public TextboxElement(LineReader reader, Element parent, CompilerArguments compilerArguments) : base(reader, parent, false, compilerArguments)
		{
		}

		protected override void OnLoad()
		{
			Name = "textbox";
			Type = ElementType.Button;
		}


		protected override void WriteCoreHtmlDefinition(StreamWriter writer)
		{
			writer.Write($"input{HtmlFormattedClasses()} type=\"text\"");
			if (HasValue)
			{
				writer.Write(" value=\"");
				ValueToHtml(writer);
				writer.Write('"');
			}
		}

		protected override bool WriteValueAutomatically => false;
	}
}
