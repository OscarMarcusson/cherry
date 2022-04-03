using System;
using System.Collections.Generic;
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
												? words.Length - argumentIndex 
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
		}
	}
}
