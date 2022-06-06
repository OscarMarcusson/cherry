using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimplifiedUserInterfaceFramework.Intermediate
{
	public enum VariableValueType
	{
		Empty,
		String,
		DynamicString,
		Integer,
		Float,
		Bool,
		Reference,
	}

	public enum Operator
	{
		Undefined,
		Add,
		Subtract,
		Multiply,
		Divide
	}

	public class VariableValue
	{
		public readonly VariableValueType Type;
		public readonly Dictionary<string, Variable> ReferencedVariables;
		public readonly string Value;
		public readonly VariableValue Left;
		public readonly Operator Operator;
		public readonly VariableValue Right;

		public VariableValue(string raw)
		{
			if (string.IsNullOrWhiteSpace(raw))
			{
				Type = VariableValueType.Empty;
				return;
			}

			raw = raw.Trim();
			if (IsStringLiteral(raw))
			{
				Type = VariableValueType.String;
				raw = raw.Length > 2
						? raw.Substring(1, raw.Length - 2)
						: ""
						;
				Value = raw;
			}
			else if(raw == "true" || raw == "false")
			{
				Type = VariableValueType.Bool;
				Value = raw;
			}
			else if(IsIntegerLiteral(raw))
			{
				Type = VariableValueType.Integer;
				Value = raw;
			}
			else if (IsFloatLiteral(raw))
			{
				Type = VariableValueType.Float;
				Value = raw.StartsWith('.')
							? "0" + raw
							: raw.EndsWith('.')
								? raw.Trim('.')
								: raw
							;
			}
			else if(raw.Contains(' '))
			{
				var values = new List<string>();
				var previousIndex = 0;
				for (int i = 0; i < raw.Length; i++)
				{
					if(raw[i] == ' ')
					{
						var value = raw.Substring(previousIndex, i - previousIndex);
						values.Add(value);

						while (raw[i] == ' ' && i < raw.Length - 1)
							i++;

						previousIndex = i;
						var next = raw[i];
					}
					else if(raw[i] == '(' || raw[i] == ')')
					{
						values.Add(raw[i].ToString());
						previousIndex = i + 1;
					}
				}

				// If we ended with a previous index under the string length it means we have to add the remainder as the last value
				if (previousIndex < raw.Length)
					values.Add(raw.Substring(previousIndex));

				RecursivelyResolveLeftRight(values, out Left, out Operator, out Right);

				// If both the left and right are integer literals we do the math here at compiler level
				if(Left.Type == VariableValueType.Integer && Right.Type == VariableValueType.Integer)
				{
					var leftInteger = int.Parse(Left.Value);
					var rightInteger = int.Parse(Right.Value);

					switch (Operator)
					{
						case Operator.Add:      Value = (leftInteger + rightInteger).ToString(); break;
						case Operator.Subtract: Value = (leftInteger - rightInteger).ToString(); break;
						case Operator.Multiply: Value = (leftInteger * rightInteger).ToString(); break;
						case Operator.Divide:   Value = (leftInteger / rightInteger).ToString(); break;
					}

					Left = null;
					Right = null;
					Operator = Operator.Undefined;
					Type = VariableValueType.Integer;
				}
				// If both the left and right are numbers with at least one being a float literal we do the math here at compiler level
				else if ((Left.Type == VariableValueType.Integer || Left.Type == VariableValueType.Float) && (Right.Type == VariableValueType.Integer || Right.Type == VariableValueType.Float))
				{
					// Do the math with the decimal type to ensure 100% precision. The minor performance hit is worth the accuracy
					var leftFloat = decimal.Parse(Left.Value);
					var rightFloat = decimal.Parse(Right.Value);

					switch (Operator)
					{
						case Operator.Add:      Value = (leftFloat + rightFloat).ToString(); break;
						case Operator.Subtract: Value = (leftFloat - rightFloat).ToString(); break;
						case Operator.Multiply: Value = (leftFloat * rightFloat).ToString(); break;
						case Operator.Divide:   Value = (leftFloat / rightFloat).ToString(); break;
					}

					Left = null;
					Right = null;
					Operator = Operator.Undefined;
					Type = VariableValueType.Float;
				}
				else if(Left.Type == VariableValueType.String || Right.Type == VariableValueType.String)
				{
					if(Operator == Operator.Multiply)
					{
						throw new NotImplementedException("String repetition not implemented yet, should be like \"a\" * 5  == \"aaaaa\"");
					}

					if (Operator != Operator.Add)
						throw new SectionException(Left.Value + ' ', Operator.ToString(), ' ' + Right.Value, "Invalid operator for string, expected +");

					Value = Left.Value + Right.Value;
					Left = null;
					Right = null;
					Operator = Operator.Undefined;
					Type = VariableValueType.String;
				}
			}
			else
			{
				Type = VariableValueType.Reference;
				Value = raw;
			}
		}


		void RecursivelyResolveLeftRight(List<string> values, out VariableValue left, out Operator leftRightOperator, out VariableValue right)
		{
			if (values.Count == 1)
			{
				throw new NotImplementedException("This really should not be possible, fixme if we somehow end up here");
			}
			else if (values.Count == 2)
			{
				throw new NotImplementedException("");
			}
			else if(values.Count == 3)
			{
				left = new VariableValue(values[0]);
				right = new VariableValue(values[2]);
				switch (values[1])
				{
					case "+": leftRightOperator = Operator.Add;       break;
					case "-": leftRightOperator = Operator.Subtract;  break;
					case "*": leftRightOperator = Operator.Multiply;  break;
					case "/": leftRightOperator = Operator.Divide;
						if((right.Type == VariableValueType.Integer && int.Parse(right.Value) == 0) || (right.Type == VariableValueType.Float && float.Parse(right.Value) == 0f))
							throw new SectionException(values[0] + ' ' + values[1], values[2], "", "Division by zero");
						break;
					default: throw new SectionException(values[0] + ' ', values[1], ' ' + values[2], "Unknown operator, expected +, -, *, or /");
				}

			}
			else
			{
				throw new NotImplementedException("More than 2 values are not supported yet");
			}
		}


		static bool IsStringLiteral(string raw)
		{
			if(raw.StartsWith('"') && raw.EndsWith('"'))
			{
				var target = raw.Length - 2;
				for(int i = 1; i < target; i++)
				{
					if (raw[i] == '"' && raw[i - 1] != '\\')
						return false;
				}
				return true;
			}

			return false;
		}

		static bool IsIntegerLiteral(string raw)
		{
			if(raw[0] == '-')
			{
				for(int i = 1; i < raw.Length; i++)
				{
					if (!char.IsDigit(raw[i]))
						return false;
				}
				return true;
			}

			for (int i = 0; i < raw.Length; i++)
			{
				if (!char.IsDigit(raw[i]))
					return false;
			}
			return true;
		}


		static bool IsFloatLiteral(string raw)
		{
			if(raw[0] == '-')
			{
				for (int i = 1; i < raw.Length; i++)
				{
					if (!char.IsDigit(raw[i]) && raw[i] != '.')
						return false;
				}
				return true;
			}

			for (int i = 0; i < raw.Length; i++)
			{
				if (!char.IsDigit(raw[i]) && raw[i] != '.')
					return false;
			}
			return true;
		}
	}
}
