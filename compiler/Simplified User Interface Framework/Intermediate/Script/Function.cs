﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SimplifiedUserInterfaceFramework.Internal.Reader;

namespace SimplifiedUserInterfaceFramework.Intermediate
{
	public class Function
	{
		public const string Declaration = "def";

		public readonly bool IsPrivate;
		public readonly string Type;
		public readonly string Name;

		public readonly FunctionArgument[] Arguments;


		public Function(string raw, int lineNumber = -1) : this(new WordReader(raw, lineNumber)) { }

		public Function(WordReader words)
		{
			if(words.First != Declaration)
				words.ThrowWordError(0, $"Invalid definition\nExpected first word to be \"def\"");

			var argumentIndex = words.IndexOf(":");

			int i = 1;
			if (words.Second == "private")
			{
				IsPrivate = true;
				i++;
			}

			var coreDefinitionLength = argumentIndex > -1 
												? argumentIndex
												: words.Length
												;

			if (coreDefinitionLength == i+1)
			{
				Type = "void";
				Name = words[i];
			}
			else if (coreDefinitionLength == i+2)
			{
				Type = words[i];
				Name = words[i+1];
			}
			else
			{
				words.ThrowWordError(i+2, $"Unknown content\n{(argumentIndex > -1 ? "The arguments are placed after the \":\" marker, which should always be placed after the function name.": "Remove, or add \":\" after the function name if these are arguments.")}", coreDefinitionLength - i - 2);
			}


			if(argumentIndex > -1)
			{
				var arguments = new List<FunctionArgument>();
				for(i = argumentIndex+1; i < words.Length; i++)
				{
					var type = words[i++];
					var name = words[i++];

					if(name == null)
						words.ThrowWordError(i, "Expected variable name");

					if (words[i+1] == "=")
						words.ThrowWordError(i + 1, "Not implemented yet");

					arguments.Add(new FunctionArgument ( type, name, null ));
				}

				if (arguments.Count > 0)
					Arguments = arguments.ToArray();
			}
		}


		public void ToJavascriptStream(StreamWriter writer, int indentation = 0)
		{
			var indentationString = indentation > 0 ? new string('\t', indentation) : "";
			writer.Write(indentationString);

			// Start
			writer.Write("function ");
			writer.Write(Name);
			if(Arguments != null)
			{
				writer.Write("(");
				writer.Write(string.Join(", ", Arguments.Select(x => x.Name)));
				writer.WriteLine(") {");
			}
			else
			{
				writer.WriteLine("() {");
			}

			// Body


			// End
			writer.Write(indentationString);
			writer.WriteLine("}");
		}
	}
}
