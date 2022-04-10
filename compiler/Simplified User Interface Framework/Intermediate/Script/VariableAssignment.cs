using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SimplifiedUserInterfaceFramework.Internal.Reader;

namespace SimplifiedUserInterfaceFramework.Intermediate
{
	public class VariableAssignment : CodeLine
	{
		public readonly string Name;
		public readonly string Operator;
		public readonly WordReader Value;

		public bool HasValue => Value != null && Value.Length > 0;


		public VariableAssignment(WordReader wordReader)
		{
			Name = wordReader.First;
			Operator = wordReader.Second;

			if (wordReader.Length > 2)
				Value = wordReader.GetWords(2);
		}


		public override void ToJavascriptStream(StreamWriter writer, int indentation = 0)
		{
			if (indentation > 0)
				writer.Write(new string('\t', indentation));

			writer.Write(Name);
			writer.Write(' ');
			writer.Write(Operator);

			if (HasValue)
			{
				writer.Write(' ');
				// TODO:: This should be resolved properly
				writer.Write(Value);
			}

			writer.WriteLine(';');
		}


		public override string ToString() => $"{Name} {Operator} {Value}";
	}
}
