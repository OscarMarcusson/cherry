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
			{
				if (IsMediaQuery(elementReader))
				{
					ParseMediaQuery(elementReader, null);
				}
				else
				{
					ParseStyle(this, elementReader);
				}
			}
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



		StyleElement ParseMediaQuery(LineReader valueReader, string name, bool isSubMediaElement = true)
		{
			var reader = new WordReader(valueReader);
			var mediaElement = new StyleElement(this, name, isMediaQuery: true);

			if (isSubMediaElement)
			{
				if (reader.First == "else")
					reader.ThrowWordError(0, "Not implemented yet");

				// TODO:: Implement proper reading with (sections & (stuff))
				for(int i = 1; i < reader.Length; i++)
				{
					switch (reader[i])
					{
						case "screen": mediaElement.DisplayLimit |= DisplayLimit.Screen; break;
						case "print":  mediaElement.DisplayLimit |= DisplayLimit.Print;  break;
						case "voice":  mediaElement.DisplayLimit |= DisplayLimit.Voice;  break;

						// Ignore for now
						case "and":
						case "&&":
						case "not":
						case "!":
							break;

						case "width":
						case "height":
							var isWidth = reader[i++] == "width";
							var comparitor = reader[i++];
							var type = reader[i].EndsWith("px")
											? "px"
											: reader[i].EndsWith("em")
												? "em"
												: "px"
												;
							var value = int.Parse(new string(reader[i].Where(x => char.IsDigit(x)).ToArray()));

							switch (comparitor)
							{
								case ">=": if (isWidth) mediaElement.MinWidth = value;   else mediaElement.MinHeight = value;     break;
								case ">":  if (isWidth) mediaElement.MinWidth = value+1; else mediaElement.MinHeight = value+1;   break;
								case "<=": if (isWidth) mediaElement.MaxWidth = value;   else mediaElement.MaxHeight = value;     break;
								case "<":  if (isWidth) mediaElement.MaxWidth = value-1; else mediaElement.MaxHeight = value - 1; break;
								case "=":  if (isWidth) mediaElement.MaxWidth = mediaElement.MinWidth = value; else mediaElement.MinHeight = mediaElement.MinHeight = value; break;
								// TODO:: Implement later, needs an OR between them
								// case "!=": mediaElement.MaxWidth = mediaElement.MinWidth = value; break;
							}

							break;

						default:
							reader.ThrowWordError(i, "Unknown query option");
							break;
					}
				}
			}

			if(name == null)
			{
				foreach (var child in valueReader.Children)
				{
					var element = ParseMediaQuery(child, child.Text, false);
					element.DisplayLimit = mediaElement.DisplayLimit;
					element.MinWidth  = mediaElement.MinWidth;
					element.MaxWidth  = mediaElement.MaxWidth;
					element.MinHeight = mediaElement.MinHeight;
					element.MaxHeight = mediaElement.MaxHeight;
				}
			}
			else
			{
				MediaQueries.Add(mediaElement);
				foreach (var child in valueReader.Children)
				{
					// TODO:: Implement
					if (IsSubtype(child)) { }
					else if (IsMediaQuery(child)) { }
				
					else mediaElement.ReadFrom(child);
				}
			}

			return mediaElement;
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
	}
}
