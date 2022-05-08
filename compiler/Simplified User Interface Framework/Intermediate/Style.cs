using SimplifiedUserInterfaceFramework.Internal.Reader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SimplifiedUserInterfaceFramework.Intermediate
{
	public enum DisplayLimit
	{
		Ignored = 0,
		Screen = 1 << 1,
		Print = 1 << 2,
		Voice = 1 << 3,
	}

	public class Style
	{
		public readonly string Name;
		public readonly bool IsGlobal;
		public readonly Dictionary<string, StyleElement> Elements = new Dictionary<string, StyleElement>();
		public List<StyleElement> MediaQueries = new List<StyleElement>();



		public Style()
		{
			Name = "Global";
			IsGlobal = true;
		}

		public Style(LineReader reader)
		{
			if(reader.Text != reader.First)
			{
				Name = reader.Text.Substring(reader.First.Length).TrimStart();
				IsGlobal = false;
			}
			else
			{
				Name = "Global";
				IsGlobal = true;
			}

			foreach(var elementReader in reader.Children)
				ParseStyle(this, elementReader);
		}



		void ParseStyle(Style style, LineReader elementReader, string parent = null, string elementName = null)
		{
			if (elementReader.Children.Count > 0)
			{
				if(elementName == null)
				{
					elementName = elementReader.Text;

					// Some types should be hard replaced to make it easier to write
					switch (elementName)
					{
						case "tab-selector":
						case ".tab-selector":
						case "ts":
						case ".ts":
							elementName = "input[type=button].tab-selector";
							break;

						case ".tab-content":
						case "tc":
						case ".tc":
							elementName = "tab-content";
							break;
					}
				}

				if (parent != null)
					elementName = parent + elementName;

				if (!Elements.TryGetValue(elementName, out var element))
					Elements[elementName] = element = new StyleElement(style, elementName);

				foreach (var valueReader in elementReader.Children)
				{
					if (IsSubtype(valueReader))
					{
						var key = valueReader.Text.Substring(1).TrimStart();

						// sub-class
						if (key.StartsWith(".")) ParseStyle(style, valueReader, elementName, key);

						// Mouse interaction (hover / click)
						else if (key == "hover")  ParseStyle(style, valueReader, elementName, ":hover");
						else if (key == "active") ParseStyle(style, valueReader, elementName, ":active");

						// Unknown
						else new WordReader(valueReader).ThrowWordError(valueReader.Text[1] == ' ' || valueReader.Text[1] == '\t' ? 1 : 0, "Unknown subtype");
					}
					else if(IsMediaQuery(valueReader))
					{
						style.ParseMediaQuery(valueReader, elementName);
					}
					// Nah, its a normal style
					else
					{
						element.ReadFrom(valueReader);
					}
				}
			}
		}



		void ParseMediaQuery(LineReader valueReader, string name)
		{
			var mediaElement = new StyleElement(this, name, isMediaQuery: true)
			{
				MaxWidth = 400
			};
			MediaQueries.Add(mediaElement);

			foreach (var child in valueReader.Children)
			{
				// TODO:: Implement
				if (IsSubtype(child)) { }
				else if (IsMediaQuery(child)) { }
				
				else mediaElement.ReadFrom(child);
			}
		}

		public void ReadFrom(Style style)
		{
			foreach(var element in style.Elements)
			{
				if (!Elements.TryGetValue(element.Key, out var existingElement))
					Elements[element.Key] = existingElement = new StyleElement(style, element.Key);

				existingElement.ReadFrom(element.Value);
			}
		}


		bool IsSubtype(LineReader reader) => reader.Text.StartsWith("#");
		bool IsMediaQuery(LineReader reader) => reader.First == "if" || reader.First == "else";



		public void ToCssStream(StreamWriter writer, int indentation)
		{
			var indent = indentation > 0 ? new string('\t', indentation) : "";
			foreach (var element in Elements)
			{
				if (element.Value.Values.Count() > 0)
					element.Value.ToCssStream(writer, indent, element.Key);
			}

			foreach (var element in MediaQueries)
			{
				if (element.Values.Count() > 0)
					element.ToCssStream(writer, indent, element.ElementName);
			}
		}























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
				foreach(var value in element.Values)
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
					if(DisplayLimit != DisplayLimit.Ignored)
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
}
