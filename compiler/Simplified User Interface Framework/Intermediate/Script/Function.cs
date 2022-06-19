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

		public readonly FunctionArgument[] Arguments;
		public readonly CodeLine[] Body;

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

			/*

			var argumentIndex = reader.Text.IndexOf(":");

			int i = 1;
			if (words.Second == "private")
			{
				IsPrivate = true;
				i++;
			}

			var coreDefinitionLength = argumentIndex > -1 
												? argumentIndex
												: words.Length
												;

			if (coreDefinitionLength == i+1)
			{
				Type = "void";
				Name = words[i];
			}
			else if (coreDefinitionLength == i+2)
			{
				Type = words[i];
				Name = words[i+1];
			}
			else
			{
				words.ThrowWordError(i+2, $"Unknown content\n{(argumentIndex > -1 ? "The arguments are placed after the \":\" marker, which should always be placed after the function name.": "Remove, or add \":\" after the function name if these are arguments.")}", coreDefinitionLength - i - 2);
			}


			if(argumentIndex > -1)
			{
				var arguments = new List<FunctionArgument>();
				for(i = argumentIndex+1; i < words.Length; i++)
				{
					var type = words[i++];
					var name = words[i++];

					if(name == null)
						words.ThrowWordError(i, "Expected variable name");

					if (words[i+1] == "=")
						words.ThrowWordError(i + 1, "Not implemented yet");

					arguments.Add(new FunctionArgument ( type, name, null ));
				}

				if (arguments.Count > 0)
					Arguments = arguments.ToArray();
			}
			*/


			// Parse body
			// var bodyBuilder = new List<CodeLine>();
			// foreach(var line in body)
			// {
			// 	if(line.First == Variable.DynamicAccessType || line.First == Variable.ReadOnlyAccessType)
			// 	{
			// 		var variable = new Variable(parentVariables, line);
			// 		// TODO:: Add to some dictionary to validate other calls with
			// 		bodyBuilder.Add(variable);
			// 	}
			// 	else if (Keywords.IsOperator(line.Second))
			// 	{
			// 		bodyBuilder.Add(new VariableAssignment(Variables, line));
			// 	}
			// 	else if(line.First == "return")
			// 	{
			// 		bodyBuilder.Add(new Return(Variables, line));
			// 	}
			// 	else if(line.Second == "(")
			// 	{
			// 		bodyBuilder.Add(new FunctionCall(Variables, line));
			// 	}
			// 	else
			// 		line.ThrowWordError(0, "Could not parse line", line.Length);
			// }
			// Body = bodyBuilder.ToArray();
		}


		internal void ThrowNameException(string error)
		{
			var index = Source.Text.IndexOf(Name);
			throw new SectionException(Source.Text.Substring(0, index), Name, Source.Text.Substring(index + Name.Length), error, Source.LineNumber);
		}


		/// <summary> Generates the body. This should be called after all variables have been resolved </summary>
		public void GenerateBody()
		{
			throw new NotImplementedException();
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
