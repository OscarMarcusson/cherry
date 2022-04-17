using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimplifiedUserInterfaceFramework.Intermediate
{
	public static class ElementParser
	{
		public static string GetName(string raw, int lineNumber = 0, string fileName = null)
		{
			if (string.IsNullOrWhiteSpace(raw))
				throw new SectionException("", " ", "", "Can't resolve element name of empty data", lineNumber, fileName);

			for(int i = 0; i < raw.Length; i++)
			{
				switch (raw[i])
				{
					case '>':
					case '.':
					case ' ':
					case '\t':
					case '=':
						if (i == 0)
							throw new SectionException("", raw[0].ToString(), raw.Length > 1 ? raw.Substring(1) : "", "Unexpected separator, expected a name", lineNumber, fileName);

						if (i >= raw.Length - 1)
							return raw;

						return raw.Substring(0, i);
				}
			}

			return raw;
		}




		public static List<string> ExtractClasses(string dataWithoutName, out string remainingDataToParse)
		{
			if(dataWithoutName == null)
			{
				remainingDataToParse = null;
				return null;
			}

			if (dataWithoutName.StartsWith("."))
			{
				for(int i = 1; i < dataWithoutName.Length; i++)
				{
					switch (dataWithoutName[i])
					{
						case ' ':
						case '\t':
						case '>':
						case '=':
							remainingDataToParse = dataWithoutName.Substring(i+1).TrimStart();
							var classes = dataWithoutName.Substring(0, i);
							return classes.Split('.', StringSplitOptions.RemoveEmptyEntries).ToList();
					}
				}
				remainingDataToParse = null;
				return dataWithoutName.Split('.', StringSplitOptions.RemoveEmptyEntries).ToList();
			}

			remainingDataToParse = dataWithoutName;
			return null;
		}





		public static int IndexOfValueStart(string line, int startAt = 0)
		{
			var index = line.IndexOf('=', startAt);
			if (index < 0)
				return index;

			var groupStart = line.LastIndexOf('(', index);
			if (groupStart > -1)
			{
				var groupEnd = line.IndexOf(')', groupStart);
				// Does the group end before this split index?
				if (groupEnd > index)
					return IndexOfValueStart(line, groupEnd);
			}

			return index;
		}
	}
}
