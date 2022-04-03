using System;
using System.Collections.Generic;
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
			while (index > -1)
			{
				nextIndex = text.IndexOfAny(SplitBy, index);
				if (nextIndex < 0)
				{
					var remainingText = text.Substring(index);
					if (remainingText.Length > 0)
						words.Add(remainingText);
					break;
				}

				index = nextIndex + 1;
			}

			Words = words.ToArray();
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
