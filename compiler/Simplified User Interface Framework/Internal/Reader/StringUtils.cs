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
	}
}
