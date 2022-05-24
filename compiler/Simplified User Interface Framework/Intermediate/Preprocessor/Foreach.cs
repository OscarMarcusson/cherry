﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimplifiedUserInterfaceFramework.Intermediate.Preprocessor
{
	public enum ForeachResourceType
	{
		Undefined = 0,
		Range,
		File
	}

	public class Foreach
	{
		public readonly string VariableName;
		public readonly ForeachResourceType ResourceType;
		public readonly string[] Values;


		public Foreach(string rawDeclaration, int lineNumber = -1, string fileName = null)
		{
			if (string.IsNullOrEmpty(rawDeclaration))
				throw new SectionException("", "", "", "Expected a foreach statement", lineNumber, fileName);

			rawDeclaration = rawDeclaration.Trim();
			
			// foreach
			if (!TryGetNextWord(rawDeclaration, 0, out var firstWord, out var index))
				throw new SectionException("", rawDeclaration, "", "Foreach statements must start with foreach, was this called incorrectly?", lineNumber, fileName);
			if (firstWord != "foreach")
				throw new SectionException("", firstWord, rawDeclaration.Substring(index), "Foreach statements must start with foreach, was this called incorrectly?", lineNumber, fileName);


			// variable
			if (!TryGetNextWord(rawDeclaration, index, out VariableName, out index))
				throw new SectionException(rawDeclaration, "", "", "Expected a variable name", lineNumber, fileName);


			// in
			ValidateNextWord(rawDeclaration, ref index, "in", "Expected an \"in\" statement after the variable", "Expected an \"in\" statement after the variable", lineNumber, fileName);


			// resource:
			index = GetEndOfSpace(rawDeclaration, index);
			var indexOfValueStart = rawDeclaration.IndexOf(':');
			if (indexOfValueStart < 0)
				throw new SectionException(rawDeclaration.Substring(0, index), rawDeclaration.Substring(index), "", "Could not find the resource marker (:)", lineNumber, fileName);

			var resourceType = rawDeclaration.Substring(index, indexOfValueStart - index);
			if (!Enum.TryParse(resourceType, true, out ResourceType) || !Enum.IsDefined(typeof(ForeachResourceType), ResourceType) || ResourceType == ForeachResourceType.Undefined)
				throw new SectionException(rawDeclaration.Substring(0, index), resourceType, rawDeclaration.Substring(indexOfValueStart), $"Unknown resource type, expected one of:\n * {string.Join("\n * ", GetResourceTypeChoices())}", lineNumber, fileName);


			// value
			switch (ResourceType)
			{
				case ForeachResourceType.Range:
					index = indexOfValueStart+1;
					var rangeSplitIndex = rawDeclaration.IndexOf('-', index);
					if(rangeSplitIndex < 0)
						throw new SectionException(rawDeclaration.Substring(0, index), rawDeclaration.Substring(index), "", $"Could not find the \"-\" range separator\nExpected a value like \"1-3\" or \"0.01-0.1\"", lineNumber, fileName);

					var leftValue = rawDeclaration.Substring(index, rangeSplitIndex - index).Trim();
					var rightValue = rawDeclaration.Substring(rangeSplitIndex + 1).Trim();
					
					if(!decimal.TryParse(leftValue, out var leftDecimalValue))
						throw new SectionException(rawDeclaration.Substring(0, index), leftValue, rawDeclaration.Substring(rangeSplitIndex), $"Could not convert \"{leftValue}\" to a number", lineNumber, fileName);
					
					if (!decimal.TryParse(rightValue, out var rightDecimalValue))
						throw new SectionException(rawDeclaration.Substring(0, rangeSplitIndex), rightValue, "", $"Could not convert \"{rightValue}\" to a number", lineNumber, fileName);

					// TODO:: Implement decimal point support, always iterate on the smallest given size (0.1 goes 0.1, 0.2, 0.3... while 0.10 goes 0.10, 0.11, 0.12...)
					var valueToAdd = rightDecimalValue > leftDecimalValue ? 1m : -1m;
					var iterations = Convert.ToInt32(Math.Abs(leftDecimalValue - rightDecimalValue)) + 1;
					Values = new string[iterations];
					for (int i = 0; i < iterations; i++)
						Values[i] = (leftDecimalValue + valueToAdd * i).ToString();
					break;

				default:
					throw new SectionException(rawDeclaration.Substring(0, index), resourceType, rawDeclaration.Substring(indexOfValueStart), "No parser exists for this resource type", lineNumber, fileName);
			}
		}



		static IEnumerable<string> GetResourceTypeChoices() => Enum.GetNames(typeof(ForeachResourceType))
																	.Where(x => x != nameof(ForeachResourceType.Undefined))
																	.Select(x => x.ToLower())
																	;



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


		static bool TryGetNextWord(string str, int index, out string word, out int nextIndex)
		{
			index = GetEndOfSpace(str, index);
			if(!TryGetNextSpace(str, index, out nextIndex))
			{
				word = null;
				return false;
			}

			word = str.Substring(index, nextIndex - index);
			return true;
		}

		static void ValidateNextWord(string str, ref int index, string expectedWord, string onNotFoundError, string onIncorrectError, int lineNumber, string fileName)
		{
			if(TryGetNextWord(str, index, out var word, out var nextIndex))
			{
				if (word != expectedWord)
					throw new SectionException(str.Substring(0, index), word, str.Substring(nextIndex), onIncorrectError, lineNumber, fileName);

				index = nextIndex;
			}
			else
			{
				throw new SectionException(str.Substring(0, index), str.Substring(nextIndex), "", onNotFoundError, lineNumber, fileName);
			}
		}
	}
}
