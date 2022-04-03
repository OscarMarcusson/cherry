using System;
using System.Collections.Generic;
using System.Text;
using SimplifiedUserInterfaceFramework.Internal.Reader;

namespace SimplifiedUserInterfaceFramework.Intermediate
{
	public enum VariableType
	{
		Dynamic,
		ReadOnly,
	}

	public class Variable
	{
		public const string ReadOnlyAccessType = "let";
		public const string DynamicAccessType = "var";

		public readonly VariableType AccessType;
		public readonly string Name;
		public readonly string Type;
		public readonly WordReader Value;


		public Variable(string raw) : this(new WordReader(raw)) { }

		public Variable(WordReader words)
		{
			switch (words.First)
			{
				case ReadOnlyAccessType:    AccessType = VariableType.ReadOnly; break;
				case DynamicAccessType:     AccessType = VariableType.Dynamic; break;

				default:
					words.ThrowWordError(0, $"Unknown variable type\nExpected {ReadOnlyAccessType} or {DynamicAccessType}");
					return;
			}

			// var a
			if(words.Length == 2)
			{
				Name = words.Second;
			}
			// var a = 12345
			else if (words.Third == "=")
			{
				Name = words.Second;
				Value = words.GetWords(3);
			}

			// var int a
			else if (words.Length == 3)
			{
				Type = words.Second;
				Name = words.Third;
			}
			// var int a = 12345
			else if (words.Fourth == "=")
			{
				Type = words.Second;
				Name = words.Third;
				Value = words.GetWords(4);
			}
			else
			{
				throw new Exception($"Could not parse name and type from {words}");
			}
		}
	}
}
