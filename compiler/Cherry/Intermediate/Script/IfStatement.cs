using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Cherry.Internal.Reader;

namespace Cherry.Intermediate
{
	public enum IfElseType
	{
		If,
		ElseIf,
		Else
	}
	public class IfStatment : CodeLine
	{
		public readonly VariableValue Condition;
		public readonly IfElseType Type;
		public readonly CodeLine[] Body;


		public IfStatment(VariablesCache parentVariables, string raw) : this(parentVariables, new LineReader(raw)) { }

		public IfStatment(VariablesCache parentVariables, LineReader reader) : base(parentVariables)
		{
			if (string.IsNullOrWhiteSpace(reader.Text))
				throw new SectionException("", "", "", "Expected \"if\", \"else if\", or \"else\"", reader.LineNumber);

			var index = 0;
			var key = reader.Text.GetNextWord(ref index);
			if(key != "if" && key != "else")
				throw new SectionException("", key, reader.Text.Substring(index - 1), "Expected \"if\", \"else if\", or \"else\"", reader.LineNumber);

			if (key == "if")
				Type = IfElseType.If;
			else
			{
				var i = index;
				var next = reader.Text.GetNextWord(ref i);
				if(next == "if")
				{
					index = i;
					Type = IfElseType.ElseIf;
				}
				else
				{
					Type = IfElseType.Else;
				}
			}


			if (index < reader.Text.Length)
			{
				if(Type == IfElseType.Else)
				{
					throw new SectionException(key, ' ' + reader.Text.Substring(index), "", "Can't use a condition for else statements\nConsider using \"else if\"", reader.LineNumber);
				}
				else
				{
					var oneLinerIndex = reader.Text.SplitCodeSection(index, ";", out var condition);

					Condition = new VariableValue(parentVariables, condition);
					if (Condition.Type != VariableValueType.Bool)
						throw new SectionException(key + ' ', condition, oneLinerIndex > 0 ? reader.Text.Substring(oneLinerIndex) : "", "Expected a boolean condition", reader.LineNumber);

					if (oneLinerIndex > 0)
					{
						if (reader.Children.Count != 0)
							throw new SectionException(reader.Text, oneLinerIndex, 1, "Can't create a one-line if statement when it has child rows", reader.LineNumber);

						index = reader.Text.GetIndexToNextNonWhitespace(oneLinerIndex + 1);
						var remainingRow = reader.Text.Substring(index);
						Body = ConvertToCodeLines(Variables, new[] { new LineReader(remainingRow, reader) });
					}
					else
					{
						Body = CodeLine.ConvertToCodeLines(Variables, reader.Children);
					}
				}
			}
			else if(Type != IfElseType.Else)
			{
				throw new SectionException(key + ' ', "", "", "Expected a boolean condition", reader.LineNumber);
			}
		}


		public override void ToJavascriptStream(StreamWriter writer, int indentation = 0)
		{
			if (indentation > 0)
				writer.Write(new string('\t', indentation));

			// TODO:: Implement javascript writing
			throw new NotImplementedException();
		}


		public override string ToString() => $"return {Condition}";
	}
}
