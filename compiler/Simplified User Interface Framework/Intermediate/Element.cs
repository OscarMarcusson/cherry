using SimplifiedUserInterfaceFramework.Internal.Reader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SimplifiedUserInterfaceFramework.Intermediate.Style;

namespace SimplifiedUserInterfaceFramework.Intermediate
{
	public class Element
	{
		public readonly Element Parent;
		public readonly List<Element> Children = new List<Element>();
		public readonly int Indent;

		public readonly string Name;
		public readonly string Type;
		public readonly string Value;
		public readonly string[] Classes;
		public readonly ValueSection[] SeparatedValues;
		public Dictionary<string, string> Events { get; private set; }

		public bool HasValue => !string.IsNullOrWhiteSpace(Value);


		public override string ToString() => Value != null ? $"{Name} = {Value}" : Name;

		public Element() { }

		public Element(LineReader reader, Element parent = null)
		{
			if(parent != null)
			{
				Parent = parent;
				parent.Children.Add(this);
				Indent = parent.Indent + 1;
			}


			var index = IndexOfValueStart(reader.Text); //  reader.Text.IndexOf('=');
			Name = index > -1 ? reader.Text.Substring(0, index).Trim() : reader.Text.Trim();
			
			if(index > -1)
			{
				Value = reader.Text.Substring(index + 1).Trim();
				if (Value.Length > 1 && Value[0] == '"' && Value[Value.Length - 1] == '"')
					Value = Value.Substring(1, Value.Length - 2);
			}

			var splitIndex = Name.IndexOf('>');

			// Macro name parsing
			if (Name.StartsWith("#"))
			{
				// TODO:: Variables? Not sure how though...
				Name = "#" + Name.Substring(1).TrimStart();
				splitIndex = Name.IndexOf('>');
			}
			// Normal name parsing
			else
			{
				index = Name.IndexOf('.');
				if (index > -1 && (splitIndex < 0 || index < splitIndex))
				{
					Classes = Name.Substring(index + 1).Split('.', StringSplitOptions.RemoveEmptyEntries);
					Name = Name.Substring(0, index);
				}
			}

			// Early split?
			if(splitIndex > -1)
			{
				var nextElement = Name.Substring(splitIndex + 1).TrimStart();
				Name = Name.Substring(0, splitIndex).TrimEnd();

				if (!string.IsNullOrWhiteSpace(Value))
					nextElement += "=" + Value;

				nextElement = new string('\t', reader.Indentation + 1) + nextElement;
				var newReader = new LineReader(nextElement);
				var child = AddChild(newReader);

				// Resolve the deepest child level, this works since the "a > b > c" logic only has one child
				while (child.Children.Count > 0)
					child = child.Children[0];

				// Create children
				foreach (var realChild in reader.Children)
					child.AddChild(realChild);

				return;
			}
			

			if (!string.IsNullOrWhiteSpace(Value))
			{
				var values = new List<ValueSection>();
				index = 0;
				while(index > -1)
				{
					var nextIndex = Value.IndexOf('{', index);
					while (nextIndex < Value.Length - 1 && Value[nextIndex + 1] == '{')
						nextIndex = Value.IndexOf('{', nextIndex + 2);

					if (nextIndex > -1)
					{
						values.Add(new ValueSection(Value.Substring(index, nextIndex - index)));
						var endIndex = Value.IndexOf('}', nextIndex);
						while (endIndex > -1 && endIndex < Value.Length - 1 && Value[endIndex + 1] == '}')
							endIndex = Value.IndexOf('}', endIndex+2);

						if(endIndex > -1)
						{
							var section = Value.Substring(nextIndex + 1, endIndex - nextIndex - 1);
							values.Add(ValueSection.ParseSection(section));
							index = endIndex + 1;
						}
						else
						{
							values.Add(new ValueSection(Value.Substring(index)));
							index = -1;
						}
					}
					else
					{
						values.Add(new ValueSection(Value.Substring(index)));
						index = -1;
					}
				}
				SeparatedValues = values.ToArray();
			}

			index = Name.IndexOf(':');
			if (index > -1)
			{
				Type = Name.Substring(index + 1).TrimStart();
				Name = Name.Substring(0, index).Trim();

				var argumentsIndex = Type.IndexOf('(');
				if (argumentsIndex > -1)
				{
					var arguments = Type.Substring(argumentsIndex + 1).Trim('(', ')', ' ', '\t');
					Type = Type.Substring(0, argumentsIndex).TrimEnd();

					ParseEvents(arguments);
				}

				Type = Type.ToLower();
			}


			// Create children
			foreach (var child in reader.Children)
				AddChild(child);
		}


		public Element AddChild(LineReader reader) => new Element(reader, this);





		static int IndexOfValueStart(string line, int startAt = 0)
		{
			var index = line.IndexOf('=', startAt);
			if (index < 0)
				return index;

			var groupStart = line.LastIndexOf('(', index);
			if (groupStart > -1)
			{
				var groupEnd = line.IndexOf(')', groupStart);
				// Does the group end before this split index?
				if (groupEnd > index)
					return IndexOfValueStart(line, groupEnd);
			}

			return index;
		}






		public void ParseEvents(string arguments)
		{
			while(TryGetNextArgument(ref arguments, out var key, out var value))
			{
				if(Events == null)
					Events = new Dictionary<string, string>();
				Events.Add(key, value);
			}
		}



		static bool TryGetNextArgument(ref string arguments, out string key, out string value)
		{
			var index = arguments.IndexOf('=');
			if(index > -1)
			{
				key = arguments.Substring(0, index);
				arguments = arguments.Substring(index + 1).TrimStart();

				if (arguments.StartsWith('"'))
				{
					index = arguments.IndexOf('"', 1);
					if(index > -1)
					{
						value = arguments.Substring(1, index - 1);
						arguments = arguments.Substring(index + 1).TrimStart();
						return true;
					}
				}

				index = arguments.IndexOf(',');
				if(index > -1)
				{
					value = arguments.Substring(0, index).TrimEnd();
					arguments = arguments.Substring(index + 1).TrimStart();
					return true;
				}

				value = arguments;
				arguments = "";
				return true;
			}
			else
			{
				key = null;
				value = null;
				return false;
			}
		}
	}
}
