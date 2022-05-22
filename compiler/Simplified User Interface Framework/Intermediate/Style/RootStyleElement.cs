using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SimplifiedUserInterfaceFramework.Internal.Reader;

namespace SimplifiedUserInterfaceFramework.Intermediate
{
	public class RootStyleElement : StyleElement
	{
		public StyleElement Hover { get; set; }
		public StyleElement Click { get; set; }
		public StyleElement Focus { get; set; }

		public List<RootStyleElement> InheritedStyles = new List<RootStyleElement>();


		public RootStyleElement(Style style, string elementName) : base(style, elementName) { }
		public RootStyleElement(Style style, LineReader reader) : base(style, reader.Text)
		{
			ParseToContent(style, reader);
		}

		private void ParseToContent(Style style, LineReader reader)
		{
			foreach (var child in reader.Children)
			{
				switch (child.First)
				{
					case "hover": Hover = new StyleElement(style, child, ElementName); break;
					case "active":
					case "click": Click = new StyleElement(style, child, ElementName); break;
					case "focus": Focus = new StyleElement(style, child, ElementName); break;

					// Hard code accept the known types for the sake of safety
					case "first-child": AddExtension(style, child, child.First); break;
					case "last-child": AddExtension(style, child, child.First); break;

					default:
						// If there is no equals sign we just assume its some css modifier
						if (!child.Text.Contains("="))
						{
							AddExtension(style, child, child.First);
						}
						// But if there is an equal sign we read everything after it as the value
						else
						{
							ReadFrom(child);
						}
						break;
				}
			}
		}

		/// <summary>
		/// See CSS specs for details, this is translated as:
		/// . == parent-name + '.' + child-name
		/// : == parent-name + ':' + child-name
		/// > == parent-name + ' ' + child-name
		/// </summary>
		void AddExtension(Style style, LineReader reader, string extension)
		{
			if (extension.StartsWith(">"))
				extension = " " + extension.Substring(1).Trim();
			else if (!extension.StartsWith(".") && !extension.StartsWith(":"))
				extension = " " + extension;

			var extensionStyle = new RootStyleElement(style, ElementName + extension);
			extensionStyle.ParseToContent(style, reader);
			InheritedStyles.Add(extensionStyle);
		}



		public void ToCssStream(StreamWriter writer, string indent)
		{
			base.ToCssStream(writer, indent);

			if (Hover != null) Hover.ToCssStream(writer, indent, $"{Hover.ElementName}:hover");
			if (Click != null) Click.ToCssStream(writer, indent, $"{Click.ElementName}:active");
			if (Focus != null) Click.ToCssStream(writer, indent, $"{Click.ElementName}:focus");

			foreach (var value in InheritedStyles)
				value.ToCssStream(writer, indent);
		}
	}
}
