using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SimplifiedUserInterfaceFramework.Internal.Reader;

namespace SimplifiedUserInterfaceFramework.Intermediate
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
				var remainder = reader.Text.Substring(index);
				if(Type == IfElseType.Else)
				{
					throw new SectionException(key, ' ' + remainder, "", "Can't use a condition on the else statement", reader.LineNumber);
				}
				else
				{
					Condition = new VariableValue(parentVariables, remainder);
					if (Condition.Type != VariableValueType.Bool)
						throw new SectionException(key + ' ', remainder, "", "Expected a boolean condition", reader.LineNumber);
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
