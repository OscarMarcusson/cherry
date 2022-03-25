using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimplifiedUserInterfaceFramework.Internal
{
	internal static class ArgumentReader
	{
		static readonly string[] Arguments = Environment.GetCommandLineArgs().Skip(1).ToArray();
		static readonly bool[] ParsedArguments = new bool[Arguments.Length];

		public static bool Exists(string key)
		{
			return RawExists($"--{key}");
		}


		public static bool Exists(string shortKey, string longKey)
		{
			if (shortKey != null && RawExists($"-{shortKey}"))
				return true;

			if (longKey != null && RawExists($"--{longKey}"))
				return true;

			return false;
		}


		static bool RawExists(string key)
		{
			for(int i = 0; i < Arguments.Length; i++)
			{
				if(Arguments[i].Equals(key, StringComparison.OrdinalIgnoreCase))
				{
					ParsedArguments[i] = true;
					return true;
				}
			}
			return false;
		}



		public static string String(string key) => String(null, key);

		public static string String(string shortKey, string longKey)
		{
			if (TryGetIndexOf(out var index, $"-{shortKey}", $"--{longKey}"))
			{
				ParsedArguments[index] = true;
				if (index < Arguments.Length - 2)
				{
					ParsedArguments[index + 1] = true;
					return Arguments[index + 1];
				}
				else
				{
					Log.Error($"Expected a value after the input argument \"{Arguments[index]}\".");
					if (index == Arguments.Length - 1)
						Log.Error(
							$"Remember that the last value is always the input file.\n" +
							$"Example solution:\n" +
							$"  {Arguments[index]} some_value \"{Last()}\"");
					Environment.Exit(1);
				}
			}

			return null;
		}


		public static T Enum<T>(string key, T defaultValue = default) where T : Enum => Enum(null, key, defaultValue);

		public static T Enum<T>(string shortKey, string longKey, T defaultValue = default) where T : Enum
		{
			var rawValue = String(shortKey, longKey);
			if(System.Enum.TryParse(typeof(T), rawValue?.ToLower(), out var parsed))
			{
				var parsedAsType = (T)parsed;
				if (System.Enum.IsDefined(typeof(T), parsedAsType))
					return parsedAsType;
			}

			return defaultValue;
		}



		public static string Last()
		{
			ParsedArguments[ParsedArguments.Length - 1] = true;
			return Arguments.Last();
		}


		public static string[] GetUnhandledArguments()
		{
			var output = new List<string>();
			for(int i = 0; i < Arguments.Length; i++)
			{
				if (!ParsedArguments[i])
					output.Add(Arguments[i]);
			}
			return output.ToArray();
		}






		#region Helpers
		static bool TryGetIndexOf(out int index, params string[] keys)
		{
			index = -1;
			for (int i = 0; i < keys.Length && index < 0; i++)
			{
				if (keys[i] != "-" && keys[i] != "--")
					index = IndexOf(keys[i]);
			}

			return index > -1;
		}


		static int IndexOf(string key)
		{
			if (key == null)
				return -1;

			for (int i = 0; i < Arguments.Length; i++)
			{
				if (Arguments[i] == key)
					return i;
			}

			return -1;
		}
		#endregion
	}
}
