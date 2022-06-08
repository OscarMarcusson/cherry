using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using SimplifiedUserInterfaceFramework.Internal.Reader;

namespace SimplifiedUserInterfaceFramework.Intermediate.Elements
{
	public class IncludeElement : Element
	{
		public string FullPath { get; set; }

		public IncludeElement(VariablesCache parentVariables, LineReader reader, Element parent, CompilerArguments compilerArguments) : base(parentVariables, reader, parent, false, compilerArguments)
		{
		}



		protected override void OnLoad()
		{
			if (Value == null)
				throw new SectionException(Source.Text, " ", "", "Expected a path value", Source.LineNumber);

			var extension = Path.GetExtension(Value);
			switch (extension)
			{
				case ".md":
				case ".markdown":
					Name = "markdown";
					break;

				// TODO:: Add more include types here

				default:
					throw new SectionException(Source.Text.Substring(0, Source.Text.Length - extension.Length + 1), extension?.Substring(1) ?? " ", "", "Expected .md or .markdown", Source.LineNumber);
			}

			FullPath = Path.Combine(CompilerArguments.RootDirectory, Value);
			if (!File.Exists(FullPath))
			{
				var i = Source.Text.LastIndexOf('=') + 1;
				for (; i < Source.Text.Length; i++)
				{
					if (Source.Text[i] != ' ' && Source.Text[i] != '\t')
						break;
				}

				var left = Source.Text.Substring(0, i);
				var value = Source.Text.Substring(i);
				throw new SectionException(left, value, "", "Could not find this file", Source.LineNumber);
			}

			var lines = File.ReadAllLines(FullPath);
			foreach(var line in lines)
			{
				if (line.StartsWith("#"))
				{
					var headingSize = 1;
					for(; headingSize < line.Length; headingSize++)
					{
						if(line[headingSize] != '#')
							break;
					}

					AddChild(new LineReader($"h{headingSize} = \"{line.Substring(headingSize).Trim()}\"", Source, Source.LineNumber));
				}
				else if(!string.IsNullOrWhiteSpace(line) && !(line.StartsWith("<!--") && line.EndsWith("-->")))
				{
					AddChild(new LineReader($"p = \"{line.Trim()}\"", Source, Source.LineNumber));
				}
			}
		}
	}
}
