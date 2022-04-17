﻿using SimplifiedUserInterfaceFramework.Intermediate.Elements;
using SimplifiedUserInterfaceFramework.Internal.Reader;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SimplifiedUserInterfaceFramework.Intermediate.Style;

namespace SimplifiedUserInterfaceFramework.Intermediate
{
	public enum ElementType
	{
		None = 0,
		Button = 1,
		Image = 2,

		Separator = 10,

		TabSelector = 100,
		TabContent  = 101,
		TabSelectorGroup  = 102,
		TabContentGroup  = 103,
	}

	public class Element
	{
		internal readonly LineReader Source;
		public readonly Element Parent;
		public readonly List<Element> Children = new List<Element>();
		public readonly int Indent;

		public string Name { get; protected set; }
		public ElementType Type { get; protected set; }
		public string Value { get; protected set; }
		public string[] Classes { get; private set; }
		public Dictionary<string,string> Configurations { get; private set; }
		public Dictionary<string, string> InlinedStyles { get; private set; }
		public Dictionary<string, string> ChildStyles { get; private set; }
		public ValueSection[] SeparatedValues { get; private set; }
		public Dictionary<string, string> Events { get; private set; }
		public string Binding { get; private set; }

		public bool HasValue => !string.IsNullOrWhiteSpace(Value);


		public override string ToString() => Value != null ? $"{Name} = {Value}" : Name;

		public Element() : this(null, null, true) { }

		public Element(LineReader reader, Element parent = null) : this(reader, parent, true) { }

		// A core constructor allows us to hide our content loading shenanigans
		protected Element(LineReader reader, Element parent, bool loadContentAutomatically)
		{
			Source = reader;

			if (parent != null)
			{
				Parent = parent;
				parent.Children.Add(this);
				Indent = parent.Indent + 1;
			}

			if (loadContentAutomatically)
				LoadContent();
		}


		protected virtual void OnLoad() { }

		private Element LoadContent()
		{
			var index = ElementParser.IndexOfValueStart(Source.Text);
			var remainingDataToParse = index > -1 ? Source.Text.Substring(0, index).Trim() : Source.Text.Trim();
			if (index > -1)
			{
				Value = Source.Text.Substring(index + 1).Trim();
				if (Value.Length > 1 && Value[0] == '"' && Value[Value.Length - 1] == '"')
					Value = Value.Substring(1, Value.Length - 2);
			}

			// If not already done, extract the name
			if(Name == null)
				Name = ElementParser.GetName(remainingDataToParse, Source.LineNumber);
			
			// Extract classes
			if(remainingDataToParse != null)
				remainingDataToParse = Name.Length < remainingDataToParse.Length ? remainingDataToParse.Substring(Name.Length).TrimStart() : null;
			var classes = ElementParser.ExtractClasses(remainingDataToParse, out remainingDataToParse);

			// Resolve the type
			if(Type == ElementType.None)
			{
				switch (Name.ToLower())
				{
					case "btn":
					case "button":
						Type = ElementType.Button;
						break;


					// TABS
					case "tab-selector":
					case "ts":
						{
							Type = ElementType.TabSelector;
							Name = "input";
							// Add class
							if (classes == null)
								classes = new List<string>();
							classes.Add("tab-selector");
						}
						break;

					case "tab-content":
					case "tc":
						Type = ElementType.TabContent;
						Name = "tab-content";
						break;

					case "tab-selector-group":
					case "tsg":
						Type = ElementType.TabSelectorGroup;
						Name = "tab-selector-group";
						break;

					case "tab-content-group":
					case "tcg":
						Type = ElementType.TabContentGroup;
						Name = "tab-content-group";
						break;

					case "---":
					case "separator":
						Type = ElementType.Separator;
						Name = "separator";
						break;
				}
			}

			// At this stage we have all the classes, apply them
			Classes = classes?.ToArray();

			// Go through all space separated configurations before getting to the value
			var isTabType = ((int)Type >= 100 && (int)Type <= 103);
			if (remainingDataToParse != null || isTabType)
			{
				Configurations = new Dictionary<string, string>();
				while(remainingDataToParse != null && remainingDataToParse.Length > 0)
				{
					index = remainingDataToParse.IndexOf(' ');
					var nextWord = index > -1 ? remainingDataToParse.Substring(0, index) : remainingDataToParse;
					switch (nextWord)
					{
						case ">":
							var nextElement = remainingDataToParse.Substring(1).TrimStart();

							if (!string.IsNullOrWhiteSpace(Value))
								nextElement += "=" + Value;

							nextElement = new string('\t', Source.Indentation + 1) + nextElement;
							var newReader = new LineReader(nextElement, lineNumber: Source.LineNumber);
							var child = AddChild(newReader);

							// Resolve the deepest child level, this works since the "a > b > c" logic only has one child
							while (child.Children.Count > 0)
								child = child.Children[0];

							// Create children
							foreach (var realChild in Source.Children)
								child.AddChild(realChild);

							// Since we've resolved inlined elements we can be certain that we are done with this element, so hard return
							goto CompleteLoad;

						default:
							index = remainingDataToParse.IndexOf('(');
							if(index > 0)
							{
								int nextIndex = 0;
								nextWord = remainingDataToParse.Substring(0, index).Trim();

								if (remainingDataToParse[index + 1] == '"')
									nextIndex = remainingDataToParse.IndexOf('"', index + 2);
								else
									nextIndex = remainingDataToParse.IndexOf(')', index);

								if (nextIndex < index)
									throw new Exception("Could not parse " + remainingDataToParse); // TODO:: PROPER ERROR

								var dataToParse = remainingDataToParse.Substring(index+1, nextIndex - index - 1).Trim('"', ' ', '\t');
								index = 0;
								remainingDataToParse = remainingDataToParse.Substring(nextIndex+1).TrimStart();

								if (remainingDataToParse.StartsWith(')'))
									remainingDataToParse = remainingDataToParse.Substring(1).TrimStart();

								if(nextWord.Length > 0)
								{
									switch (nextWord)
									{
										// case "alt":
										// 	configurations.Add($"alt={dataToParse}");
										// 	break;

										case "size":
											index = dataToParse.IndexOf(',');
											if(index > 0)
											{
												Configurations["width"] = dataToParse.Substring(0, index);
												Configurations["height"] = dataToParse.Substring(index + 1);
											}
											else
											{
												Configurations["width"] = dataToParse;
												Configurations["height"] = dataToParse;
											}
											break;

										// case "width":  configurations.Add($"{nextWord}={dataToParse}"); break;
										// case "height": configurations.Add($"{nextWord}={dataToParse}"); break;

										case "bind":
											Binding = dataToParse;
											if (!Configurations.ContainsKey("id"))
												Configurations["id"] = $"bind{Guid.NewGuid().ToString().Replace("-", "")}";
											break;

										default:
											Configurations[nextWord] = dataToParse;
											break;
									}
								}
								break;
							}
							throw new Exception("Unknown keyword: " + nextWord);
					}
				}

				// Configuration post processing for types
				switch (Type)
				{
					case ElementType.TabContentGroup:
						Configurations["id"] = "tcg::" + Value;
						break;
					case ElementType.TabSelectorGroup:
						Configurations["id"] = "tsg::" + Value;
						break;

					case ElementType.TabSelector:
						{
							if(Parent == null || Parent.Type != ElementType.TabSelectorGroup)
								throw new SectionException("", Source.First, Source.Text.Substring(Source.First.Length), "Expected a tab-selector-group parent for this type", Source.LineNumber);

							var selectorGroupId = "tsg::" + Parent.Value;
							var contentGroupId  = "tcg::"  + Parent.Value;

							if (!Configurations.TryGetValue("id", out var selectorId))
								Configurations["id"] = selectorId = $"{selectorGroupId}::{Guid.NewGuid().ToString().Replace("-","")}";

							if(!Configurations.TryGetValue("content-id", out var contentId) && !Configurations.TryGetValue("c-id", out contentId))
								throw new SectionException("", Source.Text, "", "Expected configurator content-id(\"tab-id\")", Source.LineNumber);

							Configurations["onclick"] = $"selectTab('{selectorGroupId}', '{selectorId}', '{contentGroupId}', '{contentId}')";
						}
						break;
				}

				if (Configurations.Count == 0)
					Configurations = null;
			}
			

			if (!string.IsNullOrWhiteSpace(Value))
			{
				var values = new List<ValueSection>();
				index = 0;
				while(index > -1)
				{
					var nextIndex = Value.IndexOf('{', index);
					while (nextIndex < Value.Length - 1 && Value[nextIndex + 1] == '{')
						nextIndex = Value.IndexOf('{', nextIndex + 2);

					if (nextIndex > -1)
					{
						values.Add(new ValueSection(Value.Substring(index, nextIndex - index)));
						var endIndex = Value.IndexOf('}', nextIndex);
						while (endIndex > -1 && endIndex < Value.Length - 1 && Value[endIndex + 1] == '}')
							endIndex = Value.IndexOf('}', endIndex+2);

						if(endIndex > -1)
						{
							var section = Value.Substring(nextIndex + 1, endIndex - nextIndex - 1);
							values.Add(ValueSection.ParseSection(section));
							index = endIndex + 1;
						}
						else
						{
							values.Add(new ValueSection(Value.Substring(index)));
							index = -1;
						}
					}
					else
					{
						values.Add(new ValueSection(Value.Substring(index)));
						index = -1;
					}
				}
				SeparatedValues = values.ToArray();
			}


			// Parse the child objects, either as element children or as inlined styles
			if (AddChildrenAutomatically)
			{
				foreach (var child in Source.Children)
				{
					if (child.Text.StartsWith("#"))
					{
						string rawLine;
						Dictionary<string, string> list;
						if (child.Text.Length > 1 && child.Text[1] == '#')
						{
							if (ChildStyles == null)
								ChildStyles = new Dictionary<string, string>();
							list = ChildStyles;
							rawLine = child.Text.Substring(2).TrimStart();
						}
						else
						{
							if (InlinedStyles == null)
								InlinedStyles = new Dictionary<string, string>();
							list = InlinedStyles;
							rawLine = child.Text.Substring(1).TrimStart();
						}

						index = rawLine.IndexOf('=');
						if(index > 0)
						{
							var key = rawLine.Substring(0, index).TrimEnd();
							var value = rawLine.Substring(index+1).TrimStart();
							// TODO:: Implement some duplicate check
							list[key] = value;
						}
						else
						{
							// TODO:: Better error
							throw new Exception("Could not parse inlined style, no equals sign found: " + child.Text);
						}
					}
					else
					{
						AddChild(child);
					}
				}
			}

			CompleteLoad:
			OnLoad();
			return this;
		}

		protected virtual bool AddChildrenAutomatically => true;


		public Element AddChild(LineReader reader)
		{
			if (reader.First.StartsWith("#"))
				new WordReader(reader).ThrowWordError(0, "Can't add inlined styles as a child element");

			var name = ElementParser.GetName(reader.First, reader.LineNumber);
			var index = name.IndexOf('.');
			if (index > 0 && index < name.Length-1)
				name = name.Substring(0, index);

			switch (name)
			{
				case "tabs":   return new TabsElement(reader, this) { Name = name }.LoadContent();
				case "img":
				case "image":  return new ImageElement(reader, this) { Name = "image", Type = ElementType.Image }.LoadContent();
				case "btn":
				case "button": return new ButtonElement(reader, this) { Name = "button", Type = ElementType.Button }.LoadContent();
				default:       return new Element(reader, this, false) { Name = name }.LoadContent();
			}
		}



		public void ResolveBindings(Document document)
		{
			if(Binding != null)
			{
				string id = Configurations["id"];
				if (document.Bindings.ContainsKey(id))
				{
					var displayName = Classes?.Length > 0
										? $"{Name}.{string.Join(".", Classes)}"
										: Name;

					throw new SectionException($"{displayName} ... bind(", Binding, ")", " ...", Source.LineNumber, document.Source.File);
				}

				document.Bindings[id] = Binding;
			}

			foreach (var child in Children)
				child.ResolveBindings(document);
		}



		public void ParseEvents(string arguments)
		{
			while(TryGetNextArgument(ref arguments, out var key, out var value))
			{
				if(Events == null)
					Events = new Dictionary<string, string>();
				Events.Add(key, value);
			}
		}



		static bool TryGetNextArgument(ref string arguments, out string key, out string value)
		{
			var index = arguments.IndexOf('=');
			if(index > -1)
			{
				key = arguments.Substring(0, index);
				arguments = arguments.Substring(index + 1).TrimStart();

				if (arguments.StartsWith('"'))
				{
					index = arguments.IndexOf('"', 1);
					if(index > -1)
					{
						value = arguments.Substring(1, index - 1);
						arguments = arguments.Substring(index + 1).TrimStart();
						return true;
					}
				}

				index = arguments.IndexOf(',');
				if(index > -1)
				{
					value = arguments.Substring(0, index).TrimEnd();
					arguments = arguments.Substring(index + 1).TrimStart();
					return true;
				}

				value = arguments;
				arguments = "";
				return true;
			}
			else
			{
				key = null;
				value = null;
				return false;
			}
		}




		public void ToHtmlStream(StreamWriter writer, Document document, int customIndent = -1, List<Element> customChildren = null)
		{
			var indent = ToStartHtmlStream(writer, document, customIndent: customIndent);

			// Content  (<ELEMENT>Content</ELEMENT>)
			customChildren = customChildren ?? Children;

			if (customChildren.Count() > 0)
			{
				var indentString = new string('\t', indent);
				writer.WriteLine();

				foreach (var child in customChildren)
					child.ToHtmlStream(writer, document, customIndent: indent + 1);
				writer.Write(indentString);
			}
			else
			{
				if (HasValue && Type == ElementType.None)
					this.ValueToHtml(writer);
			}

			// ...</ELEMENT>
			ToEndHtmlStream(writer);
		}



		private int ToStartHtmlStream(StreamWriter writer, Document document, int customIndent = -1)
		{
			var indentNumber = customIndent > -1 ? customIndent : Indent;
			var indent = indentNumber <= 0 ? "" : new string('\t', indentNumber);
			writer.Write(indent);
			writer.Write('<');

			WriteCoreHtmlDefinition(writer);

			// Inlined styles
			if (InlinedStyles != null || Parent?.ChildStyles != null)
			{
				writer.Write(" style=\"");

				if (InlinedStyles != null)
					InlinedStyles.ToStyleStream(writer);

				if (Parent?.ChildStyles != null)
					Parent.ChildStyles.ToStyleStream(writer);

				writer.Write('"');
			}

			// Shared configurations
			if (Configurations != null)
			{
				if (Configurations.TryGetValue("id", out var id))
					writer.Write($" id=\"{id}\"");

				if (Configurations.TryGetValue("onload", out var onLoad))
					writer.Write($" onload=\"{onLoad}\"");

				if (Configurations.TryGetValue("onclick", out var onClick))
					writer.Write($" onclick=\"{(onClick.EndsWith(")") ? onClick : $"{onClick}()")}\"");
			}

			writer.Write('>');

			return indentNumber;
		}


		protected virtual void WriteCoreHtmlDefinition(StreamWriter writer)
		{
			writer.Write(Name + HtmlFormattedClasses());
		}



		protected virtual void ToEndHtmlStream(StreamWriter writer, int customIndent = 0)
		{
			if (customIndent > 0)
				writer.Write(new string('\t', customIndent));

			writer.WriteLine($"</{Name}>");
		}



		public string HtmlFormattedClasses()
		{
			if (Classes?.Length > 0)
				return $" class=\"{string.Join(" ", Classes)}\"";

			return null;
		}
	}
}
