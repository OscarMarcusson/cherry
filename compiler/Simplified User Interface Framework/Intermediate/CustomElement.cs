using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SimplifiedUserInterfaceFramework.Internal.Reader;

namespace SimplifiedUserInterfaceFramework.Intermediate
{
	public class CustomElement
	{
		public string Name { get; set; }
		public readonly Dictionary<string, FunctionArgument> Arguments = new Dictionary<string, FunctionArgument>();
		public readonly Element RootElement;

		public CustomElement(LineReader reader, CompilerArguments compilerArguments)
		{
			var wordReader = new WordReader(reader);
			Name = wordReader.Second;
			var indexOfDot = Name.IndexOf('.');
			if (indexOfDot > 0)
				Name = Name.Substring(0, indexOfDot);
			
			if(wordReader.Length > 2)
			{
				if (wordReader.Length == 3)
					wordReader.ThrowWordError(2, (wordReader[2] == ":" ? "Argument marker without arguments" : "Unexpected word") + "\nEither remove or add arguments after");

				var arguments = wordReader.GetWords(3);
				for(int i = 0; i < arguments.Length; i++)
				{
					var type = arguments[i++];
					var name = arguments[i++];
					var argument = new FunctionArgument(type, name);
					Arguments.Add(name, argument);
					if (i >= arguments.Length)
						break;
				}
			}

			var rootReader = new LineReader(wordReader.Second, reader.Parent, reader.LineNumber);
			foreach (var child in reader.Children)
			{
				if (child.First.StartsWith("#"))
					new LineReader(child.Text, rootReader, child.LineNumber);
			}


			RootElement = new Element(rootReader, null, true, compilerArguments);
			if (RootElement.Type != ElementType.None)
				throw new SectionException("", Name, rootReader.Text.Substring(Name.Length), "Already exists as a keyword");

			foreach (var child in reader.Children)
			{
				if(!child.First.StartsWith("#"))
					child.ToElement(RootElement);
			}
		}



		public void ToJavascriptClass(StreamWriter writer, int indent = 0)
		{
			var className = Name.Replace("-", "_");
			var indentString = indent > 0 ? new string('\t', indent) : "";
			writer.WriteLine($"{indentString}class {className} {{");
			writer.WriteLine($"{indentString}\tconstructor(parent{(Arguments.Count > 0 ?  $", {string.Join(", ", Arguments.Select(x => x.Key.Replace("-", "_")))}" : "")}) {{");
			foreach(var argument in Arguments)
			{
				var name = argument.Key.Replace("-", "_");
				writer.WriteLine($"{indentString}\t\tthis.{name} = {name};");
			}

			// Dynamic element creation
			writer.WriteLine($"{indentString}\t\tthis.element = document.createElement(\"{className}\");");

			if (RootElement.Classes?.Count > 0)
				writer.WriteLine($"{indentString}\t\tthis.element.className = \"{string.Join(" ", RootElement.Classes)}\";");

			if(RootElement.InlinedStyles?.Count > 0)
				writer.WriteLine($"{indentString}\t\tthis.element.cssText  = \"{string.Join(" ", RootElement.InlinedStyles.Select(x => $"{x.Key}:{x.Value};"))}\";");

			writer.WriteLine($"{indentString}\t\tparent.appendChild(this.element);");

			// End class definition
			writer.WriteLine($"{indentString}\t}}");
			writer.WriteLine($"{indentString}}}");
		}
	}
}
