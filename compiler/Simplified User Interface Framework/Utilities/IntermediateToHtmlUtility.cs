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

			var classes = element.HtmlFormattedClasses();

			if (!string.IsNullOrWhiteSpace(element.Type))
			{
				switch (element.Type ?? "")
				{
					case "button":
						writer.Write($"input {classes}type=\"button\"");
						if (element.HasValue)
						{
							writer.Write("value=\"");
							element.ValueToHtml(writer);
							writer.Write('"');
						}
						writer.Write($" class=\"button {element.Name.Replace(',', ' ').Replace(" ", "")}\"");
						break;

					// Ignore no type
					case "":
						writer.Write(element.Name + classes);
						break;

					default:
						writer.Write($"{element.Name} {classes}type=\"{element.Type}\"");
						break;
				}
			}
			else
			{
				writer.Write(element.Name + classes);
			}

			writer.Write('>');

			return indentNumber;
		}

		public static void ToEndHtmlStream(this Intermediate.Element element, StreamWriter writer, int customIndent = 0)
		{
			if(customIndent > 0)
				writer.Write(new string('\t', customIndent));

			writer.WriteLine($"</{element.Name}>");
		}







		public static void ToRecursiveHtmlStream(this Intermediate.Element element, StreamWriter writer, Document document, int customIndent = -1, List<Intermediate.Element> customChildren = null)
		{
			if (element.Name.StartsWith("#"))
			{
				if(document.Macros.TryGetValue(element.Name, out var macro))
				{
					foreach(var macroElement in macro.Elements)
						macroElement.ToRecursiveHtmlStream(writer, document, customIndent:element.Indent+1, customChildren:element.Children);
				}
				else
				{
					// TODO:: PROPER WARNING
					throw new Exception("CANT FIND MACRO " + element.Name);
				}
			}
			else
			{
				// <ELEMENT>...
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
						child.ToRecursiveHtmlStream(writer, document, customIndent: indent+1);
					writer.Write(indentString);
				}
				else
				{
					if (element.HasValue && element.Type != "button")
						element.ValueToHtml(writer);
				}
				

				// ...</ELEMENT>
				element.ToEndHtmlStream(writer);
			}
		}
	}
}
