﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Cherry.Internal.Reader;
using Cherry.Utilities;

namespace Cherry.Intermediate
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
		public readonly VariableValueType ValueType;
		public VariableValue Value { get; set; }

		public override string ToString() => $"{(AccessType == VariableType.Dynamic ? DynamicAccessType : ReadOnlyAccessType)} {(Type != null ? $"{Type} " : "")}{Name}" + (Value != null ? $" = {Value}" : "");


		public Variable(VariablesCache parentVariables, VariableType type, string name, string value) : base(parentVariables)
		{
			if (parentVariables.Exists(name))
				throw new ArgumentException($"A variable by the name {name} already exists in this scope");
			parentVariables[name] = this;

			AccessType = type;
			Name = name;
			Value = new VariableValue(parentVariables, new WordReader(value, -1).ToString());
			ValueType = Value.Type;
			Type = ResolveTypeFromValueType(ValueType);
		}

		public Variable(VariablesCache parentVariables, string raw, int lineNumber = -1) : this(parentVariables, new WordReader(raw, lineNumber)) { }

		public Variable(VariablesCache parentVariables, WordReader words) : base(parentVariables)
		{
			switch (words.First)
			{
				case ReadOnlyAccessType: AccessType = VariableType.ReadOnly; break;
				case DynamicAccessType: AccessType = VariableType.Dynamic; break;

				default:
					words.ThrowWordError(0, $"Unknown variable type\nExpected {ReadOnlyAccessType} or {DynamicAccessType}");
					return;
			}

			int typeIndex = 0;
			int nameIndex = 0;

			// var a
			if (words.Length == 2)
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
				ValueType = Value.Type;
				Type = ResolveTypeFromValueType(ValueType);
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
				ValueType = Value.Type;
				var resolvedType = ResolveTypeFromValueType(ValueType);
				if (resolvedType != Type)
				{
					var valid = false;
					if (ValueType == VariableValueType.Integer && (resolvedType == "i32" || resolvedType == "i64")) valid = true;
					if (ValueType == VariableValueType.Float && (resolvedType == "f32" || resolvedType == "f64")) valid = true;

					if (!valid)
						throw new SectionException(words.First + ' ', Type, ' ' + words.GetWords(2).ToString(), $"Type does not match value type ({ValueType} vs {resolvedType})", words.LineNumber);
				}
			}
			else
			{
				words.ThrowWordError(1, $"Could not parse name and type", words.Length - 1);
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




		private string ResolveTypeFromValueType(VariableValueType valueType)
		{
			switch (ValueType)
			{
				case VariableValueType.DynamicString:
				case VariableValueType.String: return "string";
				case VariableValueType.Bool: return "bool";
				case VariableValueType.Integer: return "i64";
				case VariableValueType.Float: return "f64";
				case VariableValueType.Reference: throw new NotImplementedException("Automatic reference type resolving is not yet implemented");
			}
			return null;
		}





		public override void ToJavascriptStream(StreamWriter writer, int indentation = 0)
		{
			Indent(writer, indentation);

			switch (AccessType)
			{
				case VariableType.ReadOnly: writer.Write("const "); break;
				case VariableType.Dynamic: writer.Write("let "); break;
			}

			writer.Write(Name);

			if (Value != null)
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

		public override void ToCppStream(StreamWriter writer, int indentation = 0)
		{
			Indent(writer, indentation);

			if (AccessType == VariableType.ReadOnly)
				writer.Write("const ");

			writer.Write(CppType);
			writer.Write(' ');
			writer.Write(Name);

			if (Value != null)
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





		public string CppType
		{
			get
			{
				switch (Type)
				{
					// Integers
					case "i8":
					case "i16":
						return "short int";

					case "i32":
						return "int";

					case "int": // <<<< Default
					case "i64":
						return "long int";

					case "i128":
						return "long long int";


					// Unsigned integers
					case "u8":
					case "u16":
						return "unsigned short int";

					case "u32":
						return "unsigned int";

					case "uint": // <<<< Default
					case "u64":
						return "unsigned long int";

					case "u128":
						return "unsigned long long int";


					// Floats
					case "f32":
						return "float";

					case "f64":
					case "float":  // <<<< Default
						return "double";

					case "f128":
						return "long double";


					// Other builtin types
					case "bool":
						return "bool";

					case "string":
						return "string";    // TODO:: #include <string>


					default: return Type;
				}
			}
		}
	}
}
