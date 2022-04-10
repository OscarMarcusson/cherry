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

		public readonly WordReader[] Arguments;


		public FunctionCall(WordReader words)
		{
			if(words.Second != "(")
				words.ThrowWordError(1, $"Expected (");

			if(words[words.Length-1] != ")")
				words.ThrowWordError(words.Length - 1, $"Expected )");

			Name = words.First;
			Arguments = words.GetWords(2, words.Length - 3).Split(",");
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
