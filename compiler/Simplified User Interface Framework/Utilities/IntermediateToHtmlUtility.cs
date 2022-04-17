using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimplifiedUserInterfaceFramework.Intermediate;

namespace SimplifiedUserInterfaceFramework.Intermediate
{
	public static class IntermediateToHtmlUtility
	{
		public static void ToCssStream(this Style style, StreamWriter writer, int indentation)
		{
			var indent = indentation > 0 ? new string('\t', indentation) : "";
			foreach(var element in style.Elements)
			{
				if(element.Value.Values.Count() > 0)
				{
					writer.Write(indent);
					writer.Write(element.Key);
					writer.Write(' ');
					writer.WriteLine('{');

					foreach(var value in element.Value.Values)
					{
						writer.Write(indent);
						writer.Write('\t');
						writer.Write(value.Key);
						writer.Write(": ");
						writer.Write(value.Value);
						writer.WriteLine(';');
					}

					writer.Write(indent);
					writer.WriteLine('}');
				}
			}
		}




		public static void ToHtmlStream(this ValueSection[] values, StreamWriter writer)
		{
			foreach(var value in values)
			{
				if (value.IsRaw)
				{
					writer.Write(value.Text);
				}
				else
				{
					var elementName = "span";
					if(value.ConfigurationValues.TryGetValue("link", out var link))
					{
						elementName = "a";
						writer.Write("<a href=\"");
						writer.Write(link);
						writer.Write('"');
					}
					else
					{
						writer.Write("<span");
					}
					foreach(var config in value.ConfigurationValues)
					{
						switch (config.Key.ToLower())
						{
							case "style":
								writer.Write(" class=\"");
								writer.Write(config.Value);
								writer.Write('"');
								break;

								// Ignore links since that should already be handled
							case "link": break;

							default:
								throw new Exception("Could not resolve the inline configuration key for " + config.Key);
						}
					}
					writer.Write('>');
					writer.Write(value.Text);
					writer.Write($"</{elementName}>");
				}
			}
		}



		public static string HtmlFormattedClasses(this Intermediate.Element element)
		{
			if(element.Classes?.Length > 0)
				return $" class=\"{string.Join(" ", element.Classes)}\"";

			return null;
		}


		public static void ValueToHtml(this Intermediate.Element element, StreamWriter writer)
		{
			if (string.IsNullOrEmpty(element.Value))
				return;

			int i;
			var valueToPrint = element.Value;
			// Replace spaces at start with HTML formatted spaces
			for (i = 0; i < valueToPrint.Length; i++)
			{
				if (valueToPrint[i] == ' ')
					writer.Write("&nbsp;");

				else break;
			}

			if (i > 0)
				valueToPrint = valueToPrint.Substring(i);


			// Replace spaces at the end with HTML formatted spaces
			int startAt = valueToPrint.Length - 1;
			for(i = startAt; i >= 0; i--)
			{
				if (valueToPrint[i] != ' ')
					break;
			}

			if(i != startAt)
			{
				valueToPrint = valueToPrint.Substring(0, i+1);
				writer.Write(valueToPrint);

				var numberOfSpaces = startAt - i;
				for(i = 0; i < numberOfSpaces; i++)
					writer.Write("&nbsp;");
			}

			// Nothing at the end
			else
			{
				writer.Write(valueToPrint);
			}
		}


		public static int ToStartHtmlStream(this Intermediate.Element element, StreamWriter writer, Document document, int customIndent = -1)
		{
			var indentNumber = customIndent > -1 ? customIndent : element.Indent;
			var indent = indentNumber <= 0 ? "" : new string('\t', indentNumber);
			writer.Write(indent);
			writer.Write('<');

			// Core definition
			switch (element.Type)
			{
				case ElementType.Button:
				case ElementType.TabSelector:
					writer.Write($"input{element.HtmlFormattedClasses()} type=\"button\"");
					if (element.HasValue)
					{
						writer.Write(" value=\"");
						element.ValueToHtml(writer);
						writer.Write('"');
					}
					break;

				case ElementType.Image:
					writer.Write($"img src=\"{element.Value}\"{element.HtmlFormattedClasses()}");
					if(element.Configurations != null)
					{
						if (element.Configurations.TryGetValue("width",  out var width))  writer.Write($" width=\"{width}\"");
						if (element.Configurations.TryGetValue("height", out var height)) writer.Write($" height=\"{height}\"");
						if (element.Configurations.TryGetValue("alt",    out var alt))    writer.Write($" alt=\"{alt}\"");
					}
					break;

				case ElementType.None:
				case ElementType.TabContent:
				case ElementType.TabSelectorGroup:
				case ElementType.TabContentGroup:
				case ElementType.Separator:
					writer.Write(element.Name + element.HtmlFormattedClasses());
					break;

				default: throw new NotImplementedException("Has not implemented a parser for built-in type " + element.Type);
			}

			// Inlined styles
			if(element.InlinedStyles != null || element.Parent?.ChildStyles != null)
			{
				writer.Write(" style=\"");
				
				if (element.InlinedStyles != null)
					element.InlinedStyles.ToStyleStream(writer);

				if (element.Parent?.ChildStyles != null)
					element.Parent.ChildStyles.ToStyleStream(writer);
				
				writer.Write('"');
			}

			// Shared configurations
			if(element.Configurations != null)
			{
				if (element.Configurations.TryGetValue("id", out var id))
					writer.Write($" id=\"{id}\"");

				
				if (element.Configurations.TryGetValue("onload", out var onLoad))
					writer.Write($" onload=\"{onLoad}\"");

				if (element.Configurations.TryGetValue("onclick", out var onClick))
					writer.Write($" onclick=\"{(onClick.EndsWith(")") ? onClick : $"{onClick}()")}\"");
			}

			writer.Write('>');

			return indentNumber;
		}

		static void ToStyleStream(this Dictionary<string,string> values, StreamWriter writer)
		{
			foreach (var keyPair in values)
			{
				writer.Write(keyPair.Key);
				writer.Write(':');
				writer.Write(keyPair.Value);
				writer.Write(';');
			}
		}

		public static void ToEndHtmlStream(this Intermediate.Element element, StreamWriter writer, int customIndent = 0)
		{
			if(customIndent > 0)
				writer.Write(new string('\t', customIndent));

			writer.WriteLine($"</{element.Name}>");
		}







		public static void ToRecursiveHtmlStream(this Element element, StreamWriter writer, Document document, Log log, int customIndent = -1, List<Element> customChildren = null)
		{
			// TODO:: Rebuild macros
			// if(document.Macros.TryGetValue(element.Name, out var macro))
			// {
			// 	foreach(var macroElement in macro.Elements)
			// 		macroElement.ToRecursiveHtmlStream(writer, document, log, customIndent: element.Indent+1, customChildren:element.Children);
			// }
			// else
			// {
			// 	log.Error("Can't find macro " + element.Name);
			// }

			var indent = element.ToStartHtmlStream(writer, document, customIndent: customIndent);


			// Content  (<ELEMENT>Content</ELEMENT>)
			customChildren = customChildren ?? element.Children;
			
			if(customChildren.Count() > 0)
			{
				var indentString = new string('\t', indent);
				writer.WriteLine();
				// if (writeValue)
				// 	writer.WriteLine($"{indent}\t{element.Value}");

				foreach (var child in customChildren)
					child.ToRecursiveHtmlStream(writer, document, log, customIndent: indent+1);
				writer.Write(indentString);
			}
			else
			{
				if (element.HasValue && element.Type == ElementType.None)
					element.ValueToHtml(writer);
			}
			

			// ...</ELEMENT>
			element.ToEndHtmlStream(writer);
		}
	}
}
