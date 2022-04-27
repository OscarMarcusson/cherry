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

		string id;
		bool isHorizontal;
		Element listHolder;
		Element contentHolder;

		string listHolderId;
		string ListHolderId
		{
			get
			{
				if (listHolderId != null)
					return listHolderId;

				if (listHolder.Configurations == null)
					listHolder.Configurations = new Dictionary<string, string>();

				if (listHolder.Configurations.TryGetValue("id", out listHolderId))
					return listHolderId;

				listHolder.Configurations["id"] = listHolderId = $"tab-list::{Guid.NewGuid().ToString().Replace("-", "")}";
				return listHolderId;
			}
		}

		string contentHolderId;
		string ContentHolderId
		{
			get
			{
				if (contentHolderId != null)
					return contentHolderId;

				if (contentHolder.Configurations == null)
					contentHolder.Configurations = new Dictionary<string, string>();

				if (contentHolder.Configurations.TryGetValue("id", out contentHolderId))
					return contentHolderId;

				contentHolder.Configurations["id"] = contentHolderId = $"tab-content::{Guid.NewGuid().ToString().Replace("-", "")}";
				return contentHolderId;
			}
		}

		public TabsElement(LineReader reader, Element parent = null) : base(reader, parent, false) 
		{
			Name = "tabs";
			Type = ElementType.Tabs;
		}


		protected override void OnLoad()
		{
			// Load shared data
			if (Configurations == null)
				Configurations = new Dictionary<string, string>();

			if (!Configurations.TryGetValue("id", out id))
				Configurations["id"] = id = $"tabs::{Guid.NewGuid().ToString().Replace("-","")}";

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
			Children.Clear();
			ApplyClass(this);

			isHorizontal = ListAlignment == ListAlignment.Left || ListAlignment == ListAlignment.Right;

			AddStyle("display", "flex");
			switch (ListAlignment)
			{
				case ListAlignment.Left: AddStyle("flex-direction", "row"); break;
				case ListAlignment.Top: AddStyle("flex-direction", "column"); break;
				case ListAlignment.Right: AddStyle("flex-direction", "row-reverse"); break;
				case ListAlignment.Bottom: AddStyle("flex-direction", "column-reverse"); break;
			}

			AddListHolder();
			AddContentHolder();

			for(int i = 0; i < Tabs.Length; i++)
			{
				var tab = Tabs[i];
				if (tab.Configurations == null)
					tab.Configurations = new Dictionary<string, string>();

				Element listItem = null;

				if(tab.Type == ElementType.Separator)
				{
					listItem = listHolder.AddChild(new LineReader(isHorizontal ? "horizontal-separator" : "vertical-separator"));
					listItem.AddStyle(isHorizontal ? "height" : "width", "1px");
					listItem.AddStyle(isHorizontal ? "width" : "height", "100%");
					listItem.AddStyle("background-color", "black");
					ApplyClass(listItem);
				}
				else
				{
					// Shared data
					var value = tab.HasValue ? tab.Value : i.ToString();
					var name = tab.Configurations?["name"] ?? value;
					var directionClass = isHorizontal ? "vertical" : "horizontal";
					var buttonId = "tab-button::" + Guid.NewGuid().ToString().Replace("-", "");

					if (!tab.Configurations.TryGetValue("id", out var tabId))
					{
						tabId = Guid.NewGuid().ToString().Replace("-", "");
						tab.Configurations["id"] = $"tab::{tabId}";
					}


					// List item
					listItem = listHolder.AddChild(new LineReader($"tab-button.{directionClass} id({buttonId}) onclick(\"tabsSelect('{ListHolderId}','{ContentHolderId}','{buttonId}','{tabId}')\")= {name}"));
					listItem.AddStyle("-webkit-touch-callout", "none");
					listItem.AddStyle("-webkit-user-select", "none");
					listItem.AddStyle("-khtml-user-select", "none");
					listItem.AddStyle("-moz-user-select", "none");
					listItem.AddStyle("-ms-user-select", "none");
					listItem.AddStyle("user-select", "none");
					listItem.AddStyle("white-space", "nowrap");
					if(List.ChildStyles != null)
					{
						foreach (var style in List.ChildStyles)
							listItem.AddStyle(style.Key, style.Value);
					}
					ApplyClass(listItem);

					// Tab
					var content = contentHolder.AddChild(tab.Source);
					if (content.Configurations == null)
						content.Configurations = new Dictionary<string, string>();
					content.Configurations["id"] = tabId;
					ApplyClass(content);
				}

				// Shared listitem values
				listItem.AddStyle("display", isHorizontal ? "block" : "table-cell");
			}

			var startTab = listHolder.Children.FirstOrDefault(x => x.Name == "tab-button");
			if(startTab != null)
			{
				var script = $"window.addEventListener(\"load\", (event) => document.getElementById('{startTab.Configurations["id"]}').click());";
				var scriptElement = new ScriptElement(this, script);
			}
		}

		void ApplyClass(Element element)
		{
			if(HasValue)
				element.AddClass(Value);
		}




		void AddListHolder()
		{
			var className = isHorizontal ? "horizontal" : "vertical";
			listHolder = AddChild(new LineReader($"tab-list.{className}", Source));
			listHolder.AddStyle("display", "block");
			listHolder.AddStyle(isHorizontal ? "overflow-y" : "overflow-x", "auto");

			if (List.InlinedStyles != null)
			{
				foreach (var style in List.InlinedStyles)
				{
					switch (style.Key)
					{
						case "width":
							if(isHorizontal)
								listHolder.AddStyle(style.Key, style.Value);
							break;

						case "height":
							if (!isHorizontal)
								listHolder.AddStyle(style.Key, style.Value);
							break;

						// Ignore
						case "left":
						case "right":
						case "top":
						case "bottom":
						case "position":
							break;

						default:
							listHolder.AddStyle(style.Key, style.Value);
							break;
					}
				}
			}
		}


		void AddContentHolder()
		{
			contentHolder = AddChild(new LineReader("tab-content", Source));
			ApplyClass(contentHolder);
			contentHolder.AddStyle("overflow", "auto");
			

			contentHolder.AddStyle("flex-grow", "1");

			if (Content.InlinedStyles != null)
			{
				foreach (var style in Content.InlinedStyles)
				{
					switch (style.Key)
					{
						// Ignore
						case "left":
						case "right":
						case "top":
						case "bottom":
						case "width":
						case "height":
						case "position":
							break;

						default:
							contentHolder.InlinedStyles[style.Key] = style.Value;
							break;
					}
				}
			}
		}






		void CopyStyles(Element from, Element to)
		{
			if (from.InlinedStyles != null)
			{
				if (to.InlinedStyles == null)
					to.InlinedStyles = new Dictionary<string, string>();

				foreach (var style in from.InlinedStyles)
					to.InlinedStyles[style.Key] = style.Value;
			}

			if (from.ChildStyles != null)
			{
				if (to.ChildStyles == null)
					to.ChildStyles = new Dictionary<string, string>();

				foreach (var style in from.ChildStyles)
					to.ChildStyles[style.Key] = style.Value;
			}
		}


		Element AddListItem(string declaration)
		{
			var parent = listHolder;
			if (!isHorizontal)
			{
				parent = listHolder.AddChild(new LineReader("td", listHolder.Source));
				ApplyClass(parent);
			}


			var listItem = parent.AddChild(new LineReader(declaration, parent.Source));
			listItem.InlinedStyles = new Dictionary<string, string>();
			if(listHolder.ChildStyles != null)
			{
				foreach (var style in listHolder.ChildStyles)
					listItem.InlinedStyles[style.Key] = style.Value;
			}
			return listItem;
		}


		protected override bool WriteValueAutomatically => false;
	}
}
