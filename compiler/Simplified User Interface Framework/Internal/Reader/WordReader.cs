﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimplifiedUserInterfaceFramework.Internal.Reader
{
	public class WordReader
	{
		static readonly char[] SplitBy = new[] { ' ', '\t' };
		readonly string[] Words;

		public int Length => Words.Length;
		public string First     => Length > 0 ? Words[0] : null;
		public string Second    => Length > 1 ? Words[1] : null;
		public string Third     => Length > 2 ? Words[2] : null;
		public string Fourth    => Length > 3 ? Words[3] : null;
		public string Fifth     => Length > 4 ? Words[4] : null;


		public WordReader(string text)
		{
			var words = new List<string>();
			var index = 0;
			var nextIndex = 0;
			string word;
			while (index > -1)
			{
				nextIndex = text.IndexOfAny(SplitBy, index);
				word = nextIndex < 0
						? text.Substring(index)
						: text.Substring(index, nextIndex - index);

				// If this is a string we have to check for the actual end
				if (word.StartsWith('"'))
				{
					var stringEndIndex = GetEndOfStringIndex(text, index+1);
					word = text.Substring(index, stringEndIndex - index + 1);
				}

				if(word.Length > 0)
					words.Add(word);


				// Prepare the following loop, or exit if this was the last word
				if (nextIndex < 0)
					break;

				index = nextIndex + 1;
			}

			Words = words.ToArray();
		}


		int GetEndOfStringIndex(string text, int index)
		{
			while(true)
			{
				index = text.IndexOf('"', (int)index);
				if (index < 0)
					return -1;

				// If the previous index is a backslash it means we have to continue, this is not the end of the string
				if (text[index - 1] == '\\')
				{
					index++;
					continue;
				}

				// Otherwise this is it, return
				return index;
			}
		}


		public string this[int index]
		{
			get
			{
				if (index < 0 || index >= Length)
					return null;

				return Words[index];
			}
		}
	}
}
