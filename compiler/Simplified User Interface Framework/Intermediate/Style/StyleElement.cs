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
		public readonly bool IsMediaQuery;
		public readonly string ElementName;
		public DisplayLimit DisplayLimit { get; set; } = DisplayLimit.Ignored;
		public int MinWidth { get; set; }
		public int MaxWidth { get; set; }
		public int MinHeight { get; set; }
		public int MaxHeight { get; set; }
		public readonly Dictionary<string, string> Values = new Dictionary<string, string>();


		public StyleElement(Style style, string elementName, bool isMediaQuery = false)
		{
			Style = style;
			ElementName = elementName;
			IsMediaQuery = isMediaQuery;
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




		public void ToCssStream(StreamWriter writer, string indentString, string name)
		{
			if (IsMediaQuery)
			{
				writer.Write(indentString);
				writer.Write("@media ");
				if (DisplayLimit != DisplayLimit.Ignored)
				{
					var limits = new[]
						{
								DisplayLimit.HasFlag(DisplayLimit.Screen) ? "screen" : null,
								DisplayLimit.HasFlag(DisplayLimit.Print) ? "print" : null,
								DisplayLimit.HasFlag(DisplayLimit.Voice) ? "voice" : null,
							}
						.Where(x => x != null)
						;

					writer.Write(string.Join(" and ", limits));
				}

				var hasAnySize = MinWidth > 0 || MaxWidth > 0 || MinHeight > 0 || MaxHeight > 0;
				if (hasAnySize)
				{
					if (DisplayLimit != DisplayLimit.Ignored)
						writer.Write(" and");

					var sizes = new[]
						{
								MinWidth > 0 ? $"(min-width:{MinWidth}px)" : null,
								MaxWidth > 0 ? $"(max-width:{MaxWidth}px)" : null,
								MinHeight > 0 ? $"(min-height:{MinHeight}px)" : null,
								MaxHeight > 0 ? $"(max-height:{MaxHeight}px)" : null,
							}
						.Where(x => x != null)
						;
					writer.Write(string.Join(" and ", sizes));
				}
				writer.WriteLine(" {");

				WriteContent(writer, indentString + '\t', name);

				writer.Write(indentString);
				writer.WriteLine('}');
			}
			else
			{
				WriteContent(writer, indentString, name);
			}
		}


		void WriteContent(StreamWriter writer, string indentString, string name)
		{
			writer.Write(indentString);
			writer.Write(name);
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
