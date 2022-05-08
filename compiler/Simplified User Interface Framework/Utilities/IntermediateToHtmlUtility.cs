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
