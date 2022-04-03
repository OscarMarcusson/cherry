﻿using System;
using System.Collections.Generic;
using System.Text;
using SimplifiedUserInterfaceFramework.Internal.Reader;
using SimplifiedUserInterfaceFramework.Utilities;

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

			int typeIndex = 0;
			int nameIndex = 0;

			// var a
			if(words.Length == 2)
			{
				Name = words.Second;
				nameIndex = 1;
			}
			// var a = 12345
			else if (words.Third == "=")
			{
				Name = words.Second;
				nameIndex = 1;

				Value = words.GetWords(3);
			}

			// var int a
			else if (words.Length == 3)
			{
				Type = words.Second;
				typeIndex = 1;

				Name = words.Third;
				nameIndex = 2;
			}
			// var int a = 12345
			else if (words.Fourth == "=")
			{
				Type = words.Second;
				typeIndex = 1;

				Name = words.Third;
				nameIndex = 2;

				Value = words.GetWords(4);
			}
			else
			{
				words.ThrowWordError(1, $"Could not parse name and type", words.Length-1);
			}


			if(Keywords.IsKeyword(Type))
				words.ThrowWordError(typeIndex, $"Can't use reserved keywords as type");
			else if (Type == "void")
				words.ThrowWordError(typeIndex, $"A variable can't be void");

			if (Keywords.IsKeyword(Name))
				words.ThrowWordError(nameIndex, $"Invalid name, can't use reserved keywords");
		}
	}
}