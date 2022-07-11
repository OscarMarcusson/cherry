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


		public Return(VariablesCache parentVariables, LineReader reader) : base(parentVariables)
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


		public override string ToString() => $"return {Value}";
	}
}
