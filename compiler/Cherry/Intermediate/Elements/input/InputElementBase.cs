using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Cherry.Internal.Reader;

namespace Cherry.Intermediate.Elements
{
	public class InputElementBase : Element
	{
		public InputElementBase(VariablesCache parentVariables, LineReader reader, Element parent, CompilerArguments compilerArguments) : base(parentVariables, reader, parent, false, compilerArguments)
		{
		}


		protected sealed override void OnLoad()
		{
			Name = GetTypeString;
			Type = GetType;
		}


		protected virtual string GetTypeString => "";
		protected virtual ElementType GetType => ElementType.None;


		protected override string HtmlTag => "input";

		protected sealed override void WriteCoreHtmlDefinition(StreamWriter writer)
		{
			writer.Write($"{HtmlTag}{HtmlFormattedClasses()} type=\"{GetTypeString}\"");
			if (HasValue)
			{
				writer.Write(" value=\"");
				ValueToHtml(writer);
				writer.Write('"');
			}
		}

		protected sealed override bool WriteValueAutomatically => false;
	}
}
