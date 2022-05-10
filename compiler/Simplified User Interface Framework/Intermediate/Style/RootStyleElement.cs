using System;
using System.Collections.Generic;
using System.Text;
using SimplifiedUserInterfaceFramework.Internal.Reader;

namespace SimplifiedUserInterfaceFramework.Intermediate
{
	public class RootStyleElement : StyleElement
	{
		public StyleElement Hover { get; set; }
		public StyleElement Active { get; set; }

		public List<RootStyleElement> InheritedStyles = new List<RootStyleElement>();


		public RootStyleElement(Style style, string elementName) : base(style, elementName) { }
		public RootStyleElement(Style style, LineReader reader) : base(style, reader.Text) 
		{
			foreach(var child in reader.Children)
				ReadFrom(child);
		}
	}
}
