using Cherry.Internal.Reader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Cherry.Intermediate
{
	public class Style
	{
		public readonly string Name;
		public readonly bool IsGlobal;
		public readonly Dictionary<string, RootStyleElement> Elements = new Dictionary<string, RootStyleElement>();
		public Dictionary<MediaQuery, Dictionary<string, RootStyleElement>> MediaQueries = new Dictionary<MediaQuery, Dictionary<string, RootStyleElement>>();
		


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
				if (elementName == null)
					elementName = elementReader.Text;

				if (parent != null)
					elementName = parent + elementName;

				if (!Elements.TryGetValue(elementName, out var element))
					Elements[elementName] = element = new RootStyleElement(style, elementReader);

				// foreach (var valueReader in elementReader.Children)
				// {
				// 	if (IsSubtype(valueReader))
				// 	{
				// 		var key = valueReader.Text.Substring(1).TrimStart();
				// 
				// 		// sub-class
				// 		if (key.StartsWith(".")) ParseStyle(style, valueReader, elementName, key);
				// 
				// 		// Mouse interaction (hover / click)
				// 		else if (key == "hover")  ParseStyle(style, valueReader, elementName, ":hover");
				// 		else if (key == "active") ParseStyle(style, valueReader, elementName, ":active");
				// 
				// 		// Unknown
				// 		else new WordReader(valueReader).ThrowWordError(valueReader.Text[1] == ' ' || valueReader.Text[1] == '\t' ? 1 : 0, "Unknown subtype");
				// 	}
				// 	else if(IsMediaQuery(valueReader))
				// 	{
				// 		style.ParseMediaQuery(valueReader, elementName);
				// 	}
				// 	// Nah, its a normal style
				// 	else
				// 	{
				// 		element.ReadFrom(valueReader);
				// 	}
				// }
			}
		}



		StyleElement ParseMediaQuery(LineReader valueReader, string name, bool isSubMediaElement = true)
		{
			var mediaQuery = new MediaQuery(valueReader);
			if (!MediaQueries.TryGetValue(mediaQuery, out var lookup))
				MediaQueries[mediaQuery] = lookup = new Dictionary<string, RootStyleElement>();


			var mediaElement = new RootStyleElement(this, name);



			// Root media query, indent 1
			if(name == null)
			{
				foreach (var child in valueReader.Children)
				{
					var element = new RootStyleElement(this, child);

					if (lookup.TryGetValue(element.ElementName, out var existingElement))
						existingElement.ReadFrom(element);
					else 
						lookup[element.ElementName] = element;
				}
			}
			// Child media query (placed inside a regular declaration, like under a div) indent 2
			else
			{
				foreach (var child in valueReader.Children)
				{
					// TODO:: Implement
					if (IsSubtype(child)) { }
					else if (IsMediaQuery(child)) { }
				
					else
					{
						if (!lookup.TryGetValue(name, out var element))
							lookup[name] = element = mediaElement;

						element.ReadFrom(child);
					}
				}
			}

			return mediaElement;
		}

		public void ReadFrom(Style style)
		{
			foreach(var element in style.Elements)
			{
				if (!Elements.TryGetValue(element.Key, out var existingElement))
					Elements[element.Key] = existingElement = new RootStyleElement(style, element.Key);

				existingElement.ReadFrom(element.Value);
			}
		}


		bool IsSubtype(LineReader reader) => reader.Text.StartsWith("#");
		bool IsMediaQuery(LineReader reader) => reader.First == "if" || reader.First == "else";



		public void ToCssStream(StreamWriter writer, int indentation)
		{
			// Regular styles
			var indent = indentation > 0 ? new string('\t', indentation) : "";
			foreach (var element in Elements)
			{
				if (element.Value.Values.Count() > 0)
					element.Value.ToCssStream(writer, indent);
			}


			// Media queries
			var mediaIndent = indent + '\t';
			foreach (var queries in MediaQueries)
			{
				var query = queries.Key;
				var elements = queries.Value;

				if(elements.Any(x => x.Value.Values.Count() > 0))
				{
					query.ToStartCssStream(writer, indent);

					foreach(var element in elements)
					{
						if (element.Value.Values.Count() > 0)
							element.Value.ToCssStream(writer, mediaIndent);
					}

					query.ToEndCssStream(writer, indent);
				}
			}
		}
	}
}
