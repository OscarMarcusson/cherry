using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SimplifiedUserInterfaceFramework.Internal.Reader;
using SimplifiedUserInterfaceFramework.Utilities;

namespace SimplifiedUserInterfaceFramework.Intermediate
{
	public class FunctionCall : CodeLine
	{
		public readonly string Name;

		public readonly VariableValue[] Arguments;


		public FunctionCall(VariablesCache parentVariables, string raw) : this(parentVariables, new LineReader(raw)) { }

		public FunctionCall(VariablesCache parentVariables, LineReader reader) : base(parentVariables)
		{
			var opening = reader.Text.IndexOf('(');
			var closing = reader.Text.LastIndexOf(')');

			if(opening < 0)
				throw new SectionException("", reader.Text, "", "Could not find the start parentheses\nExpected \"name()\" or \"name(arguments)\"", reader.LineNumber);
			if(closing < 0)
				throw new SectionException(reader.Text, "", "", $"Expected a closing parentheses, like {Name}()", reader.LineNumber);

			if(closing < opening)
				throw new SectionException(reader.Text.Substring(0, closing), ")", reader.Text.Substring(closing+1), $"Expected a closing parentheses, like {Name}()", reader.LineNumber);

			if (opening == 0)
				throw new SectionException("", "(", reader.Text.Substring(1), "Unexpected parentheses, expected the function name first", reader.LineNumber);

			Name = reader.Text.Substring(0, opening).TrimEnd();

			if(closing > opening + 1)
			{
				var arguments = reader.Text.Substring(opening+1, closing-opening-1);
				var index = 0;
				var argumentBuilder = new List<VariableValue>();
				while(index < arguments.Length)
				{
						var splitIndex = arguments.SplitCodeSection(index, ",", out var argument);
						if(!string.IsNullOrWhiteSpace(argument))
							argumentBuilder.Add(new VariableValue(Variables, argument));

						if (splitIndex > 0)
						{
							index = splitIndex + 1;
							continue;
						}
						else
						{
							break;
						}
				}

				Arguments = argumentBuilder.ToArray();
				// TODO:: Validate types compared to the function def
			}
		}


		public override void ToJavascriptStream(StreamWriter writer, int indentation = 0)
		{
			if (indentation > 0)
				writer.Write(new string('\t', indentation));

			switch (Name)
			{
				case "print": writer.Write("console.log"); break;

				default: writer.Write(Name); break;
			}
			writer.Write('(');
			writer.Write(string.Join(", ", Arguments.Select(x => x.ToString())));
			writer.Write(')');
			writer.WriteLine(';');
		}
	}
}
