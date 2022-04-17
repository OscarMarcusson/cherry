using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimplifiedUserInterfaceFramework.Internal.Reader;

namespace SimplifiedUserInterfaceFramework.Intermediate.Elements
{
	public enum ListAlignment
	{
		Left,
		Top,
		Right,
		Bottom
	}

	public class TabsElement : Element
	{
		public Element List { get; private set; }
		public ListAlignment ListAlignment { get; private set; }
		public Element[] Tabs { get; private set; }


		public TabsElement(LineReader reader, Element parent = null) : base(reader, parent, false) { }


		protected override void OnLoad()
		{
			// Get or set default list info
			List = Children.FirstOrDefault(x => x.Name == "tab-list");
			if(List == null)
			{
				List = AddChild(new LineReader("tab-list = top"));
				ListAlignment = ListAlignment.Top;
			}
			else
			{
				if (Enum.TryParse<ListAlignment>(List.Value, true, out var listAlignment))
					ListAlignment = listAlignment;
				else
					throw new SectionException(List.Name + "... = ", List.Value, "", "Could not parse alignment, expected:\n * Left\n * Right\n * Top\n * Bottom", List.Source.LineNumber);
			}

			// Get tabs
			var tabs = new List<Element>();
			foreach(var child in Children)
			{
				switch (child.Name)
				{
					case "separator":
					case "tab":
						tabs.Add(child);
						break;

					// Ignore
					case "tab-list": break;

					default: throw new SectionException("", child.Name, "...", "Invalid for a tabs group, expected:\n * tab\n * ---\n * tab-list", child.Source.LineNumber);
				}
			}
			Tabs = tabs.ToArray();
		}
	}
}
