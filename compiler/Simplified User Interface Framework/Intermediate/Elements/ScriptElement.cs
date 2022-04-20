using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimplifiedUserInterfaceFramework.Internal.Reader;

namespace SimplifiedUserInterfaceFramework.Intermediate.Elements
{
	public class ScriptElement : Element
	{
		readonly string[] Javascript;

		internal ScriptElement(LineReader reader, Element parent) : base(reader, parent, false)
		{
			Name = "script";
			Type = ElementType.Script;

			var javascript = new List<string>();
			foreach(var child in reader.Children)
				AddJavascriptLine(javascript, reader, "");
			Javascript = javascript.ToArray();
		}


		internal ScriptElement(Element parent, string raw) : base(null, parent, false)
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
	}
}
