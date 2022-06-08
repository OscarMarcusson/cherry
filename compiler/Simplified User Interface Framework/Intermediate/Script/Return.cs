using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SimplifiedUserInterfaceFramework.Internal.Reader;

namespace SimplifiedUserInterfaceFramework.Intermediate
{
	public class Return : CodeLine
	{
		public readonly WordReader Value;

		public bool HasValue => Value != null && Value.Length > 0;


		public Return(VariablesCache parentVariables, WordReader wordReader) : base(parentVariables)
		{
			if (wordReader.Length > 1)
				Value = wordReader.GetWords(1);
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
