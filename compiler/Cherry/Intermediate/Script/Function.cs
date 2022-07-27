using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Cherry.Internal.Reader;
using Cherry.Utilities;

namespace Cherry.Intermediate
{
	public class Function : CodeLine
	{
		public const string Declaration = "def";

		public readonly bool IsPrivate;
		public readonly string Type;
		public readonly string Name;

		public readonly Variable[] Arguments;
		public CodeLine[] Body { get; private set; }

		readonly LineReader Source;


		public Function(VariablesCache parentVariables, CodeLine parent, LineReader reader) : base(parentVariables, parent)
		{
			Source = reader;
			IsPrivate = false;

			if (reader.First != Declaration)
				throw new SectionException("", reader.First, reader.First == reader.Text ? "" : reader.Text.Substring(reader.First.Length), $"Expected first word to be \"def\"", reader.LineNumber);

			var index = Declaration.Length;
			var argumentStart = reader.Text.IndexOf(':');
			var nameAndTypeString = argumentStart < 0
										? reader.Text.Substring(index)
										: reader.Text.Substring(index, argumentStart - index)
										;

			var nameAndTypeWords = nameAndTypeString.GetWords();
			if(nameAndTypeWords.Length == 1)
			{
				Type = "void";
				Name = nameAndTypeWords[0];
			}
			else if(nameAndTypeWords.Length == 2)
			{
				Type = nameAndTypeWords[0];
				Name = nameAndTypeWords[1];
			}
			else
			{
				throw new SectionException($"{Declaration} {nameAndTypeWords[0]} {nameAndTypeWords[1]} ", string.Join(" ", nameAndTypeWords.Skip(2)), argumentStart < 0 ? "" : reader.Text.Substring(argumentStart), "Unexpected words\nExpected one of:\n  def [name]\n  def [type] [name]\n  def [name] : [args]\n  def [type] [name] : [args]", reader.LineNumber);
			}


			if(argumentStart > 0)
			{
				var argumentBuilder = new List<Variable>();
				index = argumentStart + 1;
				while (index < reader.Text.Length)
				{
					// Access type is by default let (readonly)
					var accessType = VariableType.ReadOnly;
					var type = reader.Text.GetNextWord(ref index);

					// If the first word is var it means it was specifically marked as dynamic (writable). So we set the type and read the next word as type instead
					if(type == Variable.DynamicAccessType)
					{
						accessType = VariableType.Dynamic;
						type = reader.Text.GetNextWord(ref index);
					}
					// Throw on unexpected keyword
					else if (type == Variable.ReadOnlyAccessType)
					{
						throw new SectionException(reader.Text.Substring(0, index - type.Length), type, reader.Text.Substring(index-1), "Arguments are readonly by default", reader.LineNumber);
					}

					var name = reader.Text.GetNextWord(ref index, new[] { ' ', '\t', ',', '=' });

					var nextIndex = index;
					var nextWord = reader.Text.GetNextWord(ref nextIndex);
					if(nextWord == null || nextWord.StartsWith(","))
					{
						var argument = new Variable(Variables, this, $"{(accessType == VariableType.Dynamic ? Variable.DynamicAccessType : Variable.ReadOnlyAccessType)} {type} {name}", reader.LineNumber);
						argumentBuilder.Add(argument);
						index++;
					}
					else if (nextWord != null && nextWord.StartsWith("="))
					{
						throw new NotImplementedException("Default values are not yet implemented");
					}
					else if(nextWord != null)
					{
						throw new SectionException(reader.Text.Substring(0, index), nextWord, reader.Text.Substring(index+nextWord.Length), "Unknown keyword, expected \",\" or end of line", reader.LineNumber);
					}
				}

				Arguments = argumentBuilder.ToArray();
			}
		}


		internal void ThrowNameException(string error)
		{
			var index = Source.Text.IndexOf(Name);
			throw new SectionException(Source.Text.Substring(0, index), Name, Source.Text.Substring(index + Name.Length), error, Source.LineNumber);
		}


		/// <summary> Generates the body. This should be called after all variables have been resolved </summary>
		public void GenerateBody()
		{
			Body = CodeLine.ConvertToCodeLines(Variables, Source.Children, this);
		}




		public override void ToJavascriptStream(StreamWriter writer, int indentation = 0)
		{
			var indentationString = indentation > 0 ? new string('\t', indentation) : "";
			writer.Write(indentationString);

			// Start
			writer.Write("function ");
			writer.Write(Name);
			if(Arguments != null)
			{
				writer.Write("(");
				writer.Write(string.Join(", ", Arguments.Select(x => x.Name)));
				writer.WriteLine(") {");
			}
			else
			{
				writer.WriteLine("() {");
			}

			// Body
			foreach (var line in Body)
				line.ToJavascriptStream(writer, indentation + 1);

			// End
			writer.Write(indentationString);
			writer.WriteLine("}");
		}

		public override void ToCppStream(StreamWriter writer, int indentation = 0)
		{
			Indent(writer, indentation);
			writer.Write($"{TypeTranslator.ToCpp(Type)} {Name}(");
			ToCppArguments(writer);
			writer.WriteLine(") {");
			if (Body != null)
			{
				foreach (var line in Body)
					line.ToCppStream(writer, indentation + 1);
			}

			Indent(writer, indentation);
			writer.WriteLine("}");
		}



		// C++
		void ToCppArguments(StreamWriter writer)
		{
			if(Arguments?.Length > 0)
			{
				writer.Write(string.Join(", ", Arguments.Select(x => $"{x.CppType} {x.Name}")));
			}
		}

		public void ToCppForwardDeclare(StreamWriter writer)
		{
			writer.Write($"{TypeTranslator.ToCpp(Type)} {Name}(");
			ToCppArguments(writer);
			writer.WriteLine(");");
		}
	}
}
