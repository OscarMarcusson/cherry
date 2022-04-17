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
		public Element Content { get; private set; }
		public ListAlignment ListAlignment { get; private set; }
		public Element[] Tabs { get; private set; }

		Element tableListHolder;
		Element tableContentHolder;


		public TabsElement(LineReader reader, Element parent = null) : base(reader, parent, false) 
		{
			Name = "tabs";
			Type = ElementType.Tabs;
		}


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

			// Get or set the default content info
			Content = Children.FirstOrDefault(x => x.Name == "tab-content") ?? AddChild(new LineReader("tab-content"));

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
					case "tab-content":
					case "tab-list": 
						break;

					default: throw new SectionException("", child.Name, "...", "Invalid for a tabs group, expected:\n * tab\n * ---\n * tab-list", child.Source.LineNumber);
				}
			}
			Tabs = tabs.ToArray();

			// Rebuild the actual tabs hierarchy
			Name = "table";
			Children.Clear();
			AddClass("tabs");
			ApplyClass(this);

			// Add holders
			switch (ListAlignment)
			{
				case ListAlignment.Top:
					tableListHolder    = AddListHolder("tr", this);
					tableContentHolder = AddContentHolder("tr", this);
					break;

				case ListAlignment.Bottom:
					tableContentHolder = AddContentHolder("tr", this);
					tableListHolder    = AddListHolder("tr", this);
					break;

				case ListAlignment.Left:
				case ListAlignment.Right:
					var parent = AddChild(new LineReader("tr", Source));
					ApplyClass(parent);
					if (ListAlignment == ListAlignment.Left)
					{
						tableListHolder = AddListHolder("td", parent);
						tableContentHolder = AddContentHolder("td", parent);
					}
					else
					{
						tableContentHolder = AddContentHolder("td", parent);
						tableListHolder = AddListHolder("td", parent);
					}
					break;
			}

			// Create list

			// Create content
		}

		void ApplyClass(Element element)
		{
			if(HasValue)
				element.AddClass(Value);
		}

		Element AddListHolder(string wrapperName, Element parent)
		{
			var list = AddHolder(wrapperName, "list", parent);
			list.InlinedStyles = new Dictionary<string, string>();
			list.InlinedStyles["display"] = "block";
			// Copy list definition
			foreach (var style in List.InlinedStyles)
				list.InlinedStyles[style.Key] = style.Value;

			return list;
		}

		Element AddContentHolder(string wrapperName, Element parent)
		{
			var content = AddHolder(wrapperName, "content", parent);
			content.InlinedStyles = new Dictionary<string, string>();
			content.InlinedStyles["width"] = "100%";
			content.InlinedStyles["height"] = "100%";
			// Copy content definition
			foreach (var style in Content.InlinedStyles)
				content.InlinedStyles[style.Key] = style.Value;

			return content;
		}

		Element AddHolder(string wrapperName, string coreClass, Element parent)
		{
			var declaration = $"{wrapperName}.tab-{coreClass}-holder";
			var list = parent.AddChild(new LineReader(declaration, Source));
			ApplyClass(list);
			return list;
		}


		protected override bool WriteValueAutomatically => false;
	}
}
