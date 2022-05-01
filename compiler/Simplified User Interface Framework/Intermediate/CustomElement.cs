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
		public readonly Dictionary<string, Variable> Variables = new Dictionary<string, Variable>();
		public readonly Element RootElement;

		public CustomElement(LineReader reader, CompilerArguments compilerArguments)
		{
			var wordReader = new WordReader(reader);
			Name = wordReader.Second;
			var indexOfDot = Name.IndexOf('.');
			if (indexOfDot > 0)
				Name = Name.Substring(0, indexOfDot);
			
			if(wordReader.Length > 2)
				wordReader.ThrowWordError(2, "Unexpected content", wordReader.Length - 2);

			var remainingChildren = new List<LineReader>();
			remainingChildren.AddRange(reader.Children);

			// Get the variables
			for(int i = 0; i < remainingChildren.Count; i++)
			{
				if(remainingChildren[i].First == Variable.DynamicAccessType || remainingChildren[i].First == Variable.ReadOnlyAccessType)
				{
					var variable = new Variable(remainingChildren[i].Text, remainingChildren[i].LineNumber);
					if (Variables.ContainsKey(variable.Name))
						throw new SectionException(remainingChildren[i].First + ' ', (variable.Type + ' ' + variable.Name).Trim(), variable.Value != null ? $" = {variable.Value}" : "", "Already exists", remainingChildren[i].LineNumber);

					Variables[variable.Name] = variable;
					remainingChildren.RemoveAt(i--);
				}
			}

			// Create the element root
			var rootReader = new LineReader(wordReader.Second, reader.Parent, reader.LineNumber);
			for(int i = 0; i < remainingChildren.Count; i++)
			{
				if (remainingChildren[i].First.StartsWith("#"))
				{
					new LineReader(remainingChildren[i].Text, rootReader, remainingChildren[i].LineNumber);
					remainingChildren.RemoveAt(i--);
				}
			}
			RootElement = new Element(rootReader, null, true, compilerArguments);
			if (RootElement.Type != ElementType.None)
				throw new SectionException("", Name, rootReader.Text.Substring(Name.Length), "Already exists as a keyword");


			// Iterate through the remaining children and add those
			foreach (var child in remainingChildren)
				child.ToElement(RootElement);
		}



		public void ToJavascriptClass(StreamWriter writer, int indent = 0)
		{
			var className = Name.Replace("-", "_");
			var indentString = indent > 0 ? new string('\t', indent) : "";
			writer.WriteLine($"{indentString}class {className} {{");
			writer.WriteLine($"{indentString}\tconstructor(parent{(Variables.Count > 0 ?  $", {string.Join(", ", Variables.Select(x => x.Key.Replace("-", "_")))}" : "")}) {{");
			foreach(var argument in Variables)
			{
				var name = argument.Key.Replace("-", "_");
				if (argument.Value.Value != null)
				{
					writer.WriteLine($"{indentString}\t\tthis.{name} = {name} ? {name} : {argument.Value.Value};");
				}
				else
				{
					writer.WriteLine($"{indentString}\t\tthis.{name} = {name};");
				}
			}

			// Dynamic element creation
			writer.WriteLine();
			writer.WriteLine($"{indentString}\t\t// Dynamic element creation");
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
