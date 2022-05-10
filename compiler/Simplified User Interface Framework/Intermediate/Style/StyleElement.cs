using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SimplifiedUserInterfaceFramework.Internal.Reader;

namespace SimplifiedUserInterfaceFramework.Intermediate
{
	public class StyleElement
	{
		public readonly Style Style;
		public readonly string ElementName;
		public readonly Dictionary<string, string> Values = new Dictionary<string, string>();


		public StyleElement(Style style, string elementName)
		{
			Style = style;
			ElementName = elementName;
		}


		public void ReadFrom(StyleElement element)
		{
			foreach (var value in element.Values)
				Values[value.Key] = value.Value;
		}


		public void ReadFrom(LineReader reader)
		{
			var index = reader.Text.IndexOf('=');
			if (index > -1)
			{
				var valueName = reader.Text.Substring(0, index).TrimEnd().Replace(" ", "");
				var value = reader.Text.Substring(index + 1).TrimStart();
				Values[valueName] = value;
			}
		}




		public void ToCssStream(StreamWriter writer, string indentString)
		{
			writer.Write(indentString);
			writer.Write(ElementName);
			writer.Write(' ');
			writer.WriteLine('{');

			foreach (var value in Values)
			{
				writer.Write(indentString);
				writer.Write('\t');
				writer.Write(value.Key);
				writer.Write(": ");
				writer.Write(value.Value);
				writer.WriteLine(';');
			}

			writer.Write(indentString);
			writer.WriteLine('}');
		}
	}
}
