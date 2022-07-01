using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;
using System.Linq;

[assembly:InternalsVisibleTo("UnitTests")]
namespace SimplifiedUserInterfaceFramework
{
	internal static class StringUtils
	{
		public static int GetIndexToNextNonWhitespace(this string s, int index)
		{
			while (index < s.Length && s[index] == ' ')
				index++;

			return index;
		}

		public static int GetIndexToNextNonWhitespace(this string s, int index, char[] customIgnores)
		{
			while (index < s.Length && s[index] == ' ' || customIgnores.Contains(s[index]))
				index++;

			return index;
		}

		public static string GetNextWord(this string s, ref int index)
		{
			if (index >= s.Length)
				return null;

			index = s.GetIndexToNextNonWhitespace(index);
			var nextSpace = s.IndexOf(' ', index);
			if (nextSpace < 0)
			{
				var remainingWord = s.Substring(index);
				index = s.Length;
				return remainingWord;
			}

			var word = s.Substring(index, nextSpace - index);
			index = nextSpace;
			index = s.GetIndexToNextNonWhitespace(index);

			return word;
		}


		public static string[] GetWords(this string s, int index = 0)
		{
			var output = new List<string>();
			while(index < s.Length)
			{
				var word = s.GetNextWord(ref index);
				if (word != null)
					output.Add(word);
			}

			return output.ToArray();
		}

		public static string GetNextWord(this string s, ref int index, char[] splitBy)
		{
			if (index >= s.Length)
				return null;

			index = s.GetIndexToNextNonWhitespace(index, splitBy);
			var nextSpace = s.IndexOfAny(splitBy, index);
			if (nextSpace < 0)
			{
				var remainingWord = s.Substring(index);
				index = s.Length;
				return remainingWord;
			}

			var word = s.Substring(index, nextSpace - index);
			index = nextSpace;
			index = s.GetIndexToNextNonWhitespace(index);

			return word;
		}


		public static int FindEndOfString(this string s, int startIndex)
		{
			for(int i = startIndex+1; i < s.Length; i++)
			{
				if (s[i] == '"' && (i <= 0 || s[i-1] != '\\'))
					return i;
			}
			return s.Length-1;
		}




		public static readonly char[] OperatorWordSplit = new[] { ' ', '\t', '+', '-', '*', '/', '=', '!', '?', '|' };
		public static readonly char[] OperatorChars = new[] { '+', '-', '*', '/', '=', '!', '?', '|' };
		public static readonly char[] ArgumentChars = new[] { ',' };
	}
}
