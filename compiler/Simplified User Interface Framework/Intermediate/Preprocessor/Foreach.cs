using System;
using System.Collections.Generic;
using System.Text;

namespace SimplifiedUserInterfaceFramework.Intermediate.Preprocessor
{
	public class Foreach
	{
		public string VariableName { get; set; }


		public Foreach(string rawDeclaration, int lineNumber = -1, string fileName = null)
		{
			if (string.IsNullOrEmpty(rawDeclaration))
				throw new SectionException("", "", "", "Expected a foreach statement", lineNumber, fileName);

			rawDeclaration = rawDeclaration.Trim();
			if (!rawDeclaration.StartsWith("foreach"))
				throw new SectionException("", rawDeclaration, "", "Foreach statements must start with foreach, was this called incorrectly?", lineNumber, fileName);

			if(!TryGetNextSpace(rawDeclaration, 0, out var index))
				throw new SectionException(rawDeclaration, "", "", "Expected a variable name", lineNumber, fileName);

			index = GetEndOfSpace(rawDeclaration, index);
			if (!TryGetNextSpace(rawDeclaration, index, out var nextIndex))
				throw new SectionException(rawDeclaration, "", "", "Expected an \"in\" statement after the variable", lineNumber, fileName);

			VariableName = rawDeclaration.Substring(index, nextIndex - index);
		}







		static readonly char[] SplitChars = new[] { ' ', '\t' };
		static bool TryGetNextSpace(string str, int startSearchAtIndex, out int index)
		{
			startSearchAtIndex = GetEndOfSpace(str, startSearchAtIndex);

			if (startSearchAtIndex+1 >= str.Length)
			{
				index = -1;
				return false;
			}

			index = str.IndexOfAny(SplitChars, startSearchAtIndex);
			return index > -1;
		}

		static int GetEndOfSpace(string str, int index)
		{
			while (index + 1 < str.Length && (str[index] == ' ' || str[index] == '\t'))
				index++;

			return index;
		}
	}
}
