using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Cherry.Internal.Reader;

namespace Cherry.Intermediate
{
	public class Return : CodeLine
	{
		public readonly VariableValue Value;
		public bool HasValue => Value != null;


		public Return(VariablesCache parentVariables, CodeLine parent, LineReader reader) : base(parentVariables, parent)
		{
			var index = 0;
			var returnWord = reader.Text.GetNextWord(ref index);
			if (returnWord != "return")
				throw new SectionException("", returnWord, reader.Text.Substring(index - 1), "Expected \"return\"", reader.LineNumber);

			if(index < reader.Text.Length)
			{
				var remainder = reader.Text.Substring(index);
				Value = new VariableValue(parentVariables, remainder);
			}

			if(!GetFirstParentOfType<Function>(out var function))
				throw new SectionException(reader.Text, 0, 6, "Return statements can only be placed within a functions body", reader.LineNumber);
		
			if(function.Type == "void" && HasValue)
				throw new SectionException(reader.Text, index, reader.Text.Length-index, "A value can't be returned from a void function", reader.LineNumber);

			else if(function.Type != "void" && !HasValue)
				throw new SectionException("return ", "", "", "Expected a value since this is not a void function", reader.LineNumber);

			// TODO:: Check if the type matches
		}


		public override void ToJavascriptStream(StreamWriter writer, int indentation = 0)
		{
			if (indentation > 0)
				writer.Write(new string('\t', indentation));

			if (HasValue)
			{
				writer.Write("return ");
				// TODO:: This should be resolved properly
				writer.Write(Value);
				writer.WriteLine(';');
			}
			else
			{
				writer.WriteLine("return;");
			}
		}

		public override void ToCppStream(StreamWriter writer, int indentation = 0)
		{
			Indent(writer, indentation);

			if (HasValue)
			{
				writer.Write("return ");
				writer.Write(Value);
				writer.WriteLine(';');
			}
			else
			{
				writer.WriteLine("return;");
			}
		}


		public override string ToString() => $"return {Value}";
	}
}
