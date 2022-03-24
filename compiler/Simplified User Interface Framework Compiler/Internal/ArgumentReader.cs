using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimplifiedUserInterfaceFramework.Internal
{
	internal static class ArgumentReader
	{
		static readonly string[] Arguments = Environment.GetCommandLineArgs();


		public static bool Exists(string key) => Exists(null, key);

		public static bool Exists(string shortKey, string longKey)
		{
			if (shortKey != null && Arguments.Contains($"-{shortKey}"))
				return true;

			if (longKey != null && Arguments.Contains($"--{longKey}"))
				return true;

			return false;
		}



		public static string String(string key) => String(null, key);

		public static string String(string shortKey, string longKey)
		{
			if (TryGetIndexOf(out var index, $"-{shortKey}", $"--{longKey}"))
			{
				if (index < Arguments.Length - 1)
					return Arguments[index + 1];
			}

			return null;
		}


		public static T Enum<T>(string key, T defaultValue = default) where T : Enum => Enum(null, key, defaultValue);

		public static T Enum<T>(string shortKey, string longKey, T defaultValue = default) where T : Enum
		{
			var rawValue = String(shortKey, longKey);
			if(System.Enum.TryParse(typeof(T), rawValue, out var parsed))
			{
				var parsedAsType = (T)parsed;
				if (System.Enum.IsDefined(typeof(T), parsedAsType))
					return parsedAsType;
			}

			return defaultValue;
		}



		public static string Last() => Arguments.Last();






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
