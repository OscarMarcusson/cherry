using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SimplifiedUserInterfaceFramework.Intermediate
{
	public class Meta
	{
		public string Title { get; set; }
		public string Description { get; set; }
		public string Author { get; set; }
		public readonly List<string> SearchEngineKeywords = new List<string>();
		public decimal ViewPortScale { get; set; } = 1m;




		public void ToHtmlString(StreamWriter writer, int indent = 0)
		{
			var indentString = indent > 0 ? new string('\t', indent) : "";
			if(!string.IsNullOrWhiteSpace(Title))
				writer.WriteLine($"{indentString}<title>{Title}</title>");

			writer.WriteLine($"{indentString}<meta charset=\"UTF-8\">");

			if(SearchEngineKeywords.Count > 0)
				writer.WriteLine($"{indentString}<meta name=\"keywords\" content=\"{string.Join(", ", SearchEngineKeywords)}\" >");

			if (!string.IsNullOrWhiteSpace(Description))
				writer.WriteLine($"{indentString}<meta name=\"description\" content=\"{Description}\">");

			if (!string.IsNullOrWhiteSpace(Author))
				writer.WriteLine($"{indentString}<meta name=\"author\" content=\"{Author}\">");

			// <meta http-equiv="refresh" content="30">

			writer.WriteLine($"{indentString}<meta name=\"viewport\" content=\"width=device-width, initial-scale={ViewPortScale}\" >");
		}
	}
}
