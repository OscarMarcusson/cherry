using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SimplifiedUserInterfaceFramework.Internal.Reader;

namespace SimplifiedUserInterfaceFramework.Intermediate.Elements
{
	public class ButtonElement : Element
	{
		public ButtonElement(LineReader reader, Element parent = null) : base(reader, parent, false)
		{
			Name = "button";
			Type = ElementType.Button;
		}


		protected override void WriteCoreHtmlDefinition(StreamWriter writer)
		{
			writer.Write($"input{HtmlFormattedClasses()} type=\"button\"");
			if (HasValue)
			{
				writer.Write(" value=\"");
				this.ValueToHtml(writer);
				writer.Write('"');
			}
		}
	}
}
