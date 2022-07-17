using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("UnitTests")]
namespace Cherry.Internal
{
	internal class ArgumentReader
	{
		readonly string[] Arguments;
		readonly bool[] ParsedArguments;
		readonly bool ExitOnFatal;

		public Log Log { get; set; }



		/// <summary> Creates an argument reader using the application arguments. </summary>
		public ArgumentReader(bool exitOnFatal = false) : this(Environment.GetCommandLineArgs().Skip(1).ToArray(), exitOnFatal) { }

		/// <summary> Creates an argument reader with custom arguments. </summary>
		public ArgumentReader(string[] arguments, bool exitOnFatal = false)
		{
			ExitOnFatal = exitOnFatal;

			// Element level copy to make sure we are thread safe
			Arguments = new string[arguments?.Length ?? 0];
			if(Arguments.Length > 0)
				arguments.CopyTo(Arguments, 0);

			ParsedArguments = new bool[arguments.Length];
		}












		public bool Exists(string key)
		{
			return RawExists($"--{key}");
		}


		public bool Exists(string shortKey, string longKey)
		{
			if (shortKey != null && RawExists($"-{shortKey}"))
				return true;

			if (longKey != null && RawExists($"--{longKey}"))
				return true;

			return false;
		}


		bool RawExists(string key)
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



		public string String(string key) => String(null, key);

		public string String(string shortKey, string longKey)
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
					var errorMessage = $"Expected a value after the input argument \"{Arguments[index]}\".";

					if (index == Arguments.Length - 1)
						errorMessage += "\n" +
							$"Remember that the last value is always the input file.\n" +
							$"Example solution:\n" +
							$"  {Arguments[index]} some_value \"{Last()}\"";

					Log?.Error(errorMessage);

					if (ExitOnFatal)
						Environment.Exit(1);
					else
						throw new Exception(errorMessage);
				}
			}

			return null;
		}


		public T Enum<T>(string key, T defaultValue = default) where T : struct => Enum(null, key, defaultValue);

		public T Enum<T>(string shortKey, string longKey, T defaultValue = default) where T : struct
		{
			var rawValue = String(shortKey, longKey);
			if(System.Enum.TryParse<T>(rawValue, true, out var parsed))
			{
				if (System.Enum.IsDefined(typeof(T), parsed))
					return parsed;
			}

			return defaultValue;
		}



		public string Last()
		{
			if (ParsedArguments.Length == 0)
				return null;

			ParsedArguments[ParsedArguments.Length - 1] = true;
			return Arguments.Last();
		}


		public string[] GetUnhandledArguments()
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
		bool TryGetIndexOf(out int index, params string[] keys)
		{
			index = -1;
			for (int i = 0; i < keys.Length && index < 0; i++)
			{
				if (keys[i] != "-" && keys[i] != "--")
					index = IndexOf(keys[i]);
			}

			return index > -1;
		}


		int IndexOf(string key)
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
