using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Cherry.Internal.Reader;

namespace Cherry.Intermediate.Elements
{
	public class TitleElement : Element
	{
		public readonly int Heading;

		public TitleElement(VariablesCache parentVariables, LineReader reader, Element parent, CompilerArguments compilerArguments) : base(parentVariables, reader, parent, false, compilerArguments)
		{
			if(reader.First == "title")
			{
				// TODO:: Should resolve number based on section number
				Heading = 1;
			}
			else
			{
				if (reader.First.StartsWith("title_"))
				{
					var remainder = reader.First.Substring(6);
					switch (remainder)
					{
						case "1": Heading = 1; break;
						case "2": Heading = 2; break;
						case "3": Heading = 3; break;
						case "4": Heading = 4; break;
						case "5": Heading = 5; break;
						case "6": Heading = 6; break;
						default: throw new SectionException("title_", remainder, "", "Unexpected value, expected a number between 1 and 6", reader.LineNumber);
					}
				}
				else
				{
					throw new SectionException("", reader.First, "", "Expected \"title\" or \"title_1\" to \"title_6\"", reader.LineNumber);
				}
			}
		}


		protected override void OnLoad()
		{
			Name = "title";
			Type = ElementType.None;
		}



		protected override void WriteCoreHtmlDefinition(StreamWriter writer)
		{
			writer.Write($"h{Heading}{HtmlFormattedClasses()}");
		}

		internal override void ToEndHtmlStream(StreamWriter writer, int customIndent = 0)
		{
			writer.Write($"</h{Heading}>");
		}
	}
}
