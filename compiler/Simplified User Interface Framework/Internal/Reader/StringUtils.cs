using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("UnitTests")]
namespace SimplifiedUserInterfaceFramework
{
	internal static class StringUtils
	{
		public static string GetNextWord(this string s, ref int index)
		{
			if (index >= s.Length)
				return null;

			var nextSpace = s.IndexOf(' ', index);
			if (nextSpace < 0)
			{
				var remainingWord = s.Substring(index);
				index = s.Length;
				return remainingWord;
			}

			var word = s.Substring(index, nextSpace - index);
			index = nextSpace;
			while (index < s.Length && s[index] == ' ')
				index++;

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
	}
}
