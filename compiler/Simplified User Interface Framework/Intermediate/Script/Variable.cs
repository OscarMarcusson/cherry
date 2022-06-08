using System;
using System.Collections.Generic;
using System.IO;
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

	public class Variable : CodeLine
	{
		public const string ReadOnlyAccessType = "let";
		public const string DynamicAccessType = "var";

		public readonly VariableType AccessType;
		public readonly string Name;
		public readonly string Type;
		public readonly VariableValue Value;

		public override string ToString() => $"{(AccessType == VariableType.Dynamic ? DynamicAccessType : ReadOnlyAccessType)} {(Type != null ? $"{Type} " : "")}{Name}" + (Value != null ? $" = {Value}" : "");


		public Variable(VariablesCache parentVariables, VariableType type, string name, string value) : base(parentVariables)
		{
			if (parentVariables.Exists(name))
				throw new ArgumentException($"A variable by the name {name} already exists in this scope");
			parentVariables[name] = this;

			AccessType = type;
			Name = name;
			Value = new VariableValue(parentVariables, new WordReader(value, -1).ToString());
		}

		public Variable(VariablesCache parentVariables, string raw, int lineNumber = -1) : this(parentVariables, new WordReader(raw, lineNumber)) { }

		public Variable(VariablesCache parentVariables, WordReader words) : base(parentVariables)
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

				var valueWords = words.GetWords(3); ;
				Value = new VariableValue(Variables, valueWords.ToString());
				if (valueWords.Any(x => x.StartsWith('"')))
					Type = "string";
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

				Value = new VariableValue(Variables, words.GetWords(4).ToString());
			}
			else
			{
				words.ThrowWordError(1, $"Could not parse name and type", words.Length-1);
			}

			
			if (parentVariables.Exists(Name))
				throw new ArgumentException($"A variable by the name {Name} already exists in this scope");
			parentVariables[Name] = this;


			if (Keywords.IsKeyword(Type))
				words.ThrowWordError(typeIndex, $"Can't use reserved keywords as type");
			else if (Type == "void")
				words.ThrowWordError(typeIndex, $"A variable can't be void");

			if (Keywords.IsKeyword(Name))
				words.ThrowWordError(nameIndex, $"Invalid name, can't use reserved keywords");
		}



		public override void ToJavascriptStream(StreamWriter writer, int indentation = 0)
		{
			if (indentation > 0)
				writer.Write(new string('\t', indentation));

			switch (AccessType)
			{
				case VariableType.ReadOnly: writer.Write("const "); break;
				case VariableType.Dynamic: writer.Write("let "); break;
			}

			writer.Write(Name);

			if(Value != null)
			{
				writer.Write(" = ");
				writer.Write(Value);
				writer.WriteLine(";");
			}
			else
			{
				writer.WriteLine(";");
			}
		}
	}
}
