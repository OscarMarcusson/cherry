using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SimplifiedUserInterfaceFramework.Internal.Reader;

namespace SimplifiedUserInterfaceFramework.Intermediate.Elements
{
	public class InputElementBase : Element
	{
		public InputElementBase(LineReader reader, Element parent, CompilerArguments compilerArguments) : base(reader, parent, false, compilerArguments)
		{
		}


		protected sealed override void OnLoad()
		{
			Name = GetTypeString;
			Type = GetType;
		}


		protected virtual string GetTypeString => "";
		protected virtual ElementType GetType => ElementType.None;


		protected sealed override void WriteCoreHtmlDefinition(StreamWriter writer)
		{
			writer.Write($"input{HtmlFormattedClasses()} type=\"{GetTypeString}\"");
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
