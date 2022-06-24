using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SimplifiedUserInterfaceFramework.Internal.Reader;
using SimplifiedUserInterfaceFramework.Utilities;

namespace SimplifiedUserInterfaceFramework.Intermediate
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


		public Function(VariablesCache parentVariables, LineReader reader) : base(parentVariables)
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
						var argument = new Variable(Variables, $"{(accessType == VariableType.Dynamic ? Variable.DynamicAccessType : Variable.ReadOnlyAccessType)} {type} {name}", reader.LineNumber);
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
			if (Source.Children.Count == 0)
				return; // TODO:: Throw for this? A "def int max : int a, int b => a > b ? a : b" should be legal and would not really have any children. Or perhaps it would if the ctor splits after => into a child line

			var builder = new List<CodeLine>();
			foreach(var line in Source.Children)
			{
				if (line.First == Variable.ReadOnlyAccessType || line.First == Variable.DynamicAccessType)
				{
					builder.Add(new Variable(Variables, new WordReader(line)));
				}
				else if (line.First == "return")
				{
					builder.Add(new Return(Variables, line));
				}

				// Function or operator call, figure out by word 2 (a = 1 + 2 would have = as our keyword for this check)
				else
				{
					var index = 0;
					_ = line.Text.GetNextWord(ref index, StringUtils.OperatorWordSplit);
					var second = line.Text.GetNextWord(ref index);
					if(second != null && second.Length > 0 && StringUtils.OperatorChars.Contains(second[0]))
					{
						builder.Add(new VariableAssignment(Variables, line)); // TODO:: TESTS FOR THIS
					}
					else
					{
						builder.Add(new FunctionCall(Variables, new WordReader(line)));
					}
				}
			}
			Body = builder.ToArray();
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
	}
}
