using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SimplifiedUserInterfaceFramework.Internal.Reader;

namespace SimplifiedUserInterfaceFramework.Intermediate
{
	public abstract class CodeLine
	{
		public readonly VariablesCache Variables;

		public CodeLine(VariablesCache parentVariables) => Variables = new VariablesCache(parentVariables);

		public abstract void ToJavascriptStream(StreamWriter writer, int indentation = 0);






		public static CodeLine[] ConvertToCodeLines(VariablesCache variables, IEnumerable<LineReader> children)
		{
			if (children.Count() == 0)
				return null; // TODO:: Throw for this? A "def int max : int a, int b => a > b ? a : b" should be legal and would not really have any children. Or perhaps it would if the ctor splits after => into a child line

			var builder = new List<CodeLine>();
			foreach (var line in children)
			{
				if (line.First == Variable.ReadOnlyAccessType || line.First == Variable.DynamicAccessType)
				{
					builder.Add(new Variable(variables, new WordReader(line)));
				}
				else if (line.First == "return")
				{
					builder.Add(new Return(variables, line));
				}

				// Function or operator call, figure out by word 2 (a = 1 + 2 would have = as our keyword for this check)
				else
				{
					var index = 0;
					_ = line.Text.GetNextWord(ref index, StringUtils.OperatorWordSplit);
					var second = line.Text.GetNextWord(ref index);
					if (second != null && second.Length > 0 && StringUtils.OperatorChars.Contains(second[0]))
					{
						builder.Add(new VariableAssignment(variables, line)); // TODO:: TESTS FOR THIS
					}
					else
					{
						builder.Add(new FunctionCall(variables, new WordReader(line)));
					}
				}
			}
			return builder.ToArray();
		}
	}
}
