﻿using System;
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

		public List<RootStyleElement> InheritedStyles = new List<RootStyleElement>();


		public RootStyleElement(Style style, string elementName) : base(style, elementName) { }
		public RootStyleElement(Style style, LineReader reader) : base(style, reader.Text) 
		{
			foreach(var child in reader.Children)
			{
				if (child.First == "hover")
				{
					Hover = new StyleElement(style, child, ElementName);
				}
				else if(child.First == "active" || child.First == "click") // Include "active" for easier CSS transition
				{
					Click = new StyleElement(style, child, ElementName);
				}
				else if (!child.Text.Contains("="))
				{
					var childStyle = new RootStyleElement(style, child);
					InheritedStyles.Add(childStyle);
				}
				else
				{
					ReadFrom(child);
				}
			}
		}



		public void ToCssStream(StreamWriter writer, string indent)
		{
			base.ToCssStream(writer, indent);

			if (Hover != null) Hover.ToCssStream(writer, indent, $"{Hover.ElementName}:hover");
			if (Click != null) Click.ToCssStream(writer, indent, $"{Click.ElementName}:active");
		}
	}
}