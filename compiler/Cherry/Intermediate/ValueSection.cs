using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cherry.Intermediate
{
	public class ValueSection
	{
		public readonly Dictionary<string, string> ConfigurationValues;
		public readonly string Text;
		public bool IsRaw => ConfigurationValues == null;
		public bool HasText => Text != null;


		public ValueSection(string text)
		{
			Text = text;
			ConfigurationValues = null;
		}

		private ValueSection(string configuration, string text)
		{
			ConfigurationValues = new Dictionary<string,string>();
			Text = text;

			for(int i = 0; i  < configuration.Length; i++)
			{
				if(configuration[i] == '=')
				{
					var key = configuration.Substring(0, i).Trim();
					string value = null;

					configuration = configuration.Substring(i+1);
					i = 0;

					if(configuration[0] == '"')
					{
						while(++i < configuration.Length)
						{
							if(configuration[i] == '"')
							{
								value = configuration.Substring(1, i - 1);
								configuration = configuration.Substring(i + 1);
								i = 0;
								break;
							}
							else if(configuration[i] == '\\')
							{
								if (i + 2 < configuration.Length && configuration[i] == '"')
									i++;
							}
						}
					}
					else
					{
						i = configuration.IndexOf(' ');
						value = i > 0 ? configuration.Substring(0, i) : configuration;
					}

					ConfigurationValues.Add(key.ToLower(), value);
				}
			}
		}




		public static ValueSection ParseSection(string section) => ParseSection(section, 0);

		static ValueSection ParseSection(string section, int startSectionBreakSearchAtIndex)
		{
			var nextSectionBreak = section.IndexOf(':', startSectionBreakSearchAtIndex);
			if(section.LastIndexOf('"', nextSectionBreak) > -1)
			{
				var stringStart = NextStringMarker(section, 0);
				var stringEnd = NextStringMarker(section, stringStart + 1);

				while(stringStart > -1 && stringEnd > -1 && stringEnd < nextSectionBreak)
				{
					stringStart = NextStringMarker(section, stringEnd+1);
					stringEnd = NextStringMarker(section, stringStart + 1);
				}

				if (stringStart < nextSectionBreak && stringEnd > nextSectionBreak)
					return ParseSection(section, stringEnd);
			}

			var config = section.Substring(0, nextSectionBreak).TrimEnd();
			var text = section.Substring(nextSectionBreak + 1);
			return new ValueSection(config, text);
		}


		static int GetEndOfNextString(string section, int startAt = 0)
		{
			var nextString = section.IndexOf('"', startAt);
			if(nextString > -1)
				nextString = section.IndexOf('"', nextString + 1);

			return nextString;
		}

		static int NextStringMarker(string section, int startAt = 0) => section.IndexOf('"', startAt);



		public override string ToString() => Text;
	}
}
