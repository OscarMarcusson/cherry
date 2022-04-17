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

		
		public static void ToStyleStream(this Dictionary<string,string> values, StreamWriter writer)
		{
			foreach (var keyPair in values)
			{
				writer.Write(keyPair.Key);
				writer.Write(':');
				writer.Write(keyPair.Value);
				writer.Write(';');
			}
		}
	}
}
