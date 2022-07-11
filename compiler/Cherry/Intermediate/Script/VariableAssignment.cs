using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Cherry.Internal.Reader;

namespace Cherry.Intermediate
{
	public class VariableAssignment : CodeLine
	{
		public readonly Variable Variable;
		public readonly string Name;
		public readonly string Operator;
		public readonly VariableValue Value;
		public readonly bool WasCompileTimeEvaluated;

		public bool HasValue => Value != null;


		public VariableAssignment(VariablesCache parentVariables, string raw) : this(parentVariables, new LineReader(raw)) { }

		public VariableAssignment(VariablesCache parentVariables, LineReader reader) : base(parentVariables)
		{
			var index = 0;
			Name = reader.Text.GetNextWord(ref index, StringUtils.OperatorWordSplit);
			if (!Variables.TryGetVariableRecursive(Name, out Variable))
				throw new SectionException("", Name, " " + reader.Text.Substring(index), "Could not find a variable with that name", reader.LineNumber);

			if(Variable.AccessType != VariableType.Dynamic)
				throw new SectionException("", Name, " " + reader.Text.Substring(index), "Can't update a readonly variable", reader.LineNumber);

			var operatorStart = index;
			while (index < reader.Text.Length && StringUtils.OperatorChars.Contains(reader.Text[index]))
				index++;

			Operator = reader.Text.Substring(operatorStart, index - operatorStart);
			Value = new VariableValue(Variables, reader.Text.Substring(index));
			// If the variable is already a literal and the new value is also a literal we calculate it now to avoid runtime costs
			// For example:
			//   var a = 1
			//   a += 1
			// Is understood as "var a = 2", and the += 1 assignment will never be done at runtime
			if (Value.IsLiteral && Variable.Value.IsLiteral)
			{
				WasCompileTimeEvaluated = true;
				Variable.Value = new VariableValue(Variable.Value, Value, OperatorExtensions.Parse(Operator), true);
			}
			else if (Value.Type != Variable.ValueType && Variable.ValueType != VariableValueType.String)
			{
				throw new SectionException(reader.Text.Substring(0, index), reader.Text.Substring(index), "", "Type does not match variable type", reader.LineNumber);
			}
		}


		public override void ToJavascriptStream(StreamWriter writer, int indentation = 0)
		{
			if (indentation > 0)
				writer.Write(new string('\t', indentation));

			writer.Write(Name);
			writer.Write(' ');
			writer.Write(Operator);

			if (HasValue)
			{
				writer.Write(' ');
				// TODO:: This should be resolved properly
				writer.Write(Value);
			}

			writer.WriteLine(';');
		}


		public override string ToString() => $"{Name} {Operator} {Value}";
	}
}
