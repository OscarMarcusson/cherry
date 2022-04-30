using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SimplifiedUserInterfaceFramework.Internal.Reader;

namespace SimplifiedUserInterfaceFramework.Intermediate.Elements
{
	public class ScriptElement : Element
	{
		readonly string[] Javascript;

		internal ScriptElement(LineReader reader, Element parent, CompilerArguments compilerArguments) : base(reader, parent, false, compilerArguments)
		{
			Name = "script";
			Type = ElementType.Script;

			var javascript = new List<string>();
			foreach(var child in reader.Children)
				AddJavascriptLine(javascript, reader, "");
			Javascript = javascript.ToArray();
		}


		internal ScriptElement(Element parent, string raw, CompilerArguments compilerArguments) : base(null, parent, false, compilerArguments)
		{
			Name = "script";
			Type = ElementType.Script;
			Javascript = raw
							.Split('\n')
							.Select(x => x.TrimEnd('\r', ' ', '\t'))
							.ToArray()
							;
		}


		void AddJavascriptLine(List<string> lines, LineReader reader, string prefix)
		{
			if (!string.IsNullOrWhiteSpace(reader.Text))
				lines.Add(prefix + reader.Text);

			if(reader.Children.Count > 0)
			{
				prefix += '\t';
				foreach(var line in reader.Children)
					AddJavascriptLine(lines, line, prefix);
			}
		}


		protected override void WriteContentToHtml(StreamWriter writer, Document document, int indent, List<Element> children)
		{
			// If we have a single row we simply inline it like <script>/*code goes here*/</script>
			if(Javascript.Length == 1)
			{
				writer.Write(Javascript[0]);
			}
			else if(Javascript.Length > 1)
			{
				var indentString = new string('\t', indent + 1);
				writer.WriteLine("");
				writer.Write(indentString);
				writer.WriteLine(string.Join(writer.NewLine + new string('\t', indent + 1), Javascript));
				writer.Write(new string('\t', indent));
			}
		}
	}
}
