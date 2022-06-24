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

	public static class OperatorExtensions
	{
		public static Operator Parse(string rawOperator)
		{
			switch (rawOperator)
			{
				case "+":
				case "+=":
					return Operator.Add;

				case "-":
				case "-=":
					return Operator.Subtract;

				case "*":
				case "*=":
					return Operator.Multiply;

				case "/":
				case "/=":
					return Operator.Divide;

				default:
					return Operator.Undefined;
			}
		}
	}

	public class VariableValue
	{
		public VariableValueType Type { get; private set; }
		public Variable ReferencedVariable { get; private set; }
		public string Value { get; private set; }
		public VariableValue Left { get; private set; }
		public Operator Operator { get; private set; }
		public VariableValue Right { get; private set; }
		public bool IsLiteral { get; private set; }


		public VariableValue(VariablesCache parentVariables, string raw)
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
				IsLiteral = true;
			}
			else if(raw == "true" || raw == "false")
			{
				Type = VariableValueType.Bool;
				Value = raw;
				IsLiteral = true;
			}
			else if(IsIntegerLiteral(raw))
			{
				Type = VariableValueType.Integer;
				Value = raw;
				IsLiteral = true;
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
				IsLiteral = true;
			}
			else if(raw.Contains(' '))
			{
				var values = new List<string>();
				var previousIndex = 0;
				for (int i = 0; i < raw.Length; i++)
				{
					if (raw[i] == ' ' || raw[i] == '(' || raw[i] == ')')
					{
						if (i != previousIndex)
						{
							var value = raw.Substring(previousIndex, i - previousIndex);
							values.Add(value);
						}

						if (raw[i] == '(' || raw[i] == ')')
						{
							values.Add(raw[i].ToString());
							i++;
							while (i < raw.Length - 1 && raw[i] == ' ')
								i++;
						}
						else
						{
							while (i < raw.Length - 1 && raw[i] == ' ')
								i++;
						}

						previousIndex = i--;
					}
				}

				// If we ended with a previous index under the string length it means we have to add the remainder as the last value
				if (previousIndex < raw.Length)
					values.Add(raw.Substring(previousIndex));

				// TODO:: Move to using variables directly inside func?
				RecursivelyResolveLeftRight(parentVariables, values, out var left, out var op, out var right);
				Left = left;
				Operator = op;
				Right = right;
				ResolveLeftRightLiterals();
			}
			else
			{
				if(parentVariables.TryGetVariableRecursive(raw, out var variable))
				{
					if(variable.AccessType == VariableType.ReadOnly)
					{
						CopyFrom(variable.Value);
					}
					else
					{
						Type = VariableValueType.Reference;
						ReferencedVariable = variable;
						Value = null;
					}
				}
				// else if ()     check for things like namespaces or member variables, like book.first_page, person.name, or math.pi
				else
				{
					throw new SectionException("", raw, "", $"Could not find a variable called \"{raw}\"");
				}
			}
		}

		public VariableValue(VariableValue left, VariableValue right, Operator operatorType)
		{
			Left = left;
			Right = right;
			Operator = operatorType;
			if (Left.IsLiteral && Right.IsLiteral)
			{
				ResolveLeftRightLiterals();
			}
			else
			{
				if (left.Type == right.Type)
				{
					Type = left.Type;
				}
				else if (left.Type == VariableValueType.String || right.Type == VariableValueType.String)
				{
					Type = VariableValueType.String;
				}
				else if ((left.Type == VariableValueType.Integer && right.Type == VariableValueType.Float) || (left.Type == VariableValueType.Float && right.Type == VariableValueType.Integer))
				{
					Type = VariableValueType.Float;
				}
				else
				{
					throw new NotImplementedException($"No combination exists for {left.Type} and {right.Type}");
				}
			}
		}

		private void ResolveLeftRightLiterals()
		{
			// If we don't have a right for whatever reason we just use the lefts content as our own
			if (Right == null)
			{
				if (Left != null)
				{
					CopyFrom(Left);
				}
			}
			// If both the left and right are integer literals we do the math here at compiler level
			else if (Left.Type == VariableValueType.Integer && Right.Type == VariableValueType.Integer)
			{
				CombineIntegerLiteral();
			}
			// If both the left and right are numbers with at least one being a float literal we do the math here at compiler level
			else if ((Left.Type == VariableValueType.Integer || Left.Type == VariableValueType.Float) && (Right.Type == VariableValueType.Integer || Right.Type == VariableValueType.Float))
			{
				CombineFloatLiteral();
			}
			else if (Left.Type == VariableValueType.String || Right.Type == VariableValueType.String)
			{
				CombineAsStringLiteral();
			}
		}

		private void CombineIntegerLiteral()
		{
			var leftInteger = int.Parse(Left.Value);
			var rightInteger = int.Parse(Right.Value);

			switch (Operator)
			{
				case Operator.Add: Value = (leftInteger + rightInteger).ToString(); break;
				case Operator.Subtract: Value = (leftInteger - rightInteger).ToString(); break;
				case Operator.Multiply: Value = (leftInteger * rightInteger).ToString(); break;
				case Operator.Divide: Value = (leftInteger / rightInteger).ToString(); break;
			}

			Left = null;
			Right = null;
			Operator = Operator.Undefined;
			Type = VariableValueType.Integer;
			IsLiteral = true;
		}

		private void CombineFloatLiteral()
		{
			// Do the math with the decimal type to ensure 100% precision. The minor performance hit is worth the accuracy
			var leftFloat = decimal.Parse(Left.Value);
			var rightFloat = decimal.Parse(Right.Value);

			switch (Operator)
			{
				case Operator.Add: Value = (leftFloat + rightFloat).ToString(); break;
				case Operator.Subtract: Value = (leftFloat - rightFloat).ToString(); break;
				case Operator.Multiply: Value = (leftFloat * rightFloat).ToString(); break;
				case Operator.Divide: Value = (leftFloat / rightFloat).ToString(); break;
			}

			Left = null;
			Right = null;
			Operator = Operator.Undefined;
			Type = VariableValueType.Float;
			IsLiteral = true;
		}

		private void CombineAsStringLiteral()
		{
			// Multiplied string concatination, useful when generating indentations or other repeated sequences
			if (Operator == Operator.Multiply)
			{
				if ((Left.Type == VariableValueType.String && Right.Type == VariableValueType.Integer) || (Left.Type == VariableValueType.Integer && Right.Type == VariableValueType.String))
				{
					string valueToRepeat;
					int integer;

					if (Left.Type == VariableValueType.String)
					{
						valueToRepeat = Left.Value;
						integer = int.Parse(Right.Value);
						if (integer < 0)
							throw new SectionException($"{valueToRepeat} * ", integer.ToString(), "", "Multiplied string concatination can't be done with a negative value");
					}
					else
					{
						valueToRepeat = Right.Value;
						integer = int.Parse(Left.Value);
						if (integer < 0)
							throw new SectionException("", integer.ToString(), $" * {valueToRepeat}", "Multiplied string concatination can't be done with a negative value");
					}

					if (integer == 0)
					{
						Value = "";
					}
					else
					{
						Value = valueToRepeat;
						for (int i = 1; i < integer; i++)
							Value += valueToRepeat;
					}
				}
				else
				{
					var error = Left.Type == VariableValueType.String && Right.Type == VariableValueType.String
									? "Can't multiply two strings, but a string and an integer can be multiplied"
									: "Multiplied string concatination requires one string and one integer"
									;
					throw new SectionException("", $"{Left.Value} * {Right.Value}", "", error);
				}
			}
			// Normal string concatination
			else if (Operator == Operator.Add)
			{
				Value = Left.Value + Right.Value;
			}
			// User error
			else
				throw new SectionException(Left.Value + ' ', Operator.ToString(), ' ' + Right.Value, "Invalid operator for string, expected +");

			Left = null;
			Right = null;
			Operator = Operator.Undefined;
			Type = VariableValueType.String;
			IsLiteral = true;
		}

		

		#region Combining
		internal void Combine(VariableValue value, Operator operatorType)
		{
			// if (operatorString == "=")
			// 	CopyFrom(value);
			// else
			// {
			// 	operatorString = operatorString.Trim('=');
			// 	var newValue = new VariableValue(variables, $"{Value} {operatorString} {value.Value}");
			// 	CopyFrom(newValue);
			// }
			switch (operatorType)
			{
				case Operator.Add: Add(value); break;
				case Operator.Subtract:  Subtract(value);break;
				case Operator.Multiply:  Multiply(value);break;
				case Operator.Divide:  Divide(value);break;
				default: throw new SectionException("", operatorType.ToString(), "", "Operator not found");
			}
		}

		internal void Add(VariableValue value)
		{
			if (IsLiteral && value.IsLiteral)
			{
				switch (Type)
				{
					case VariableValueType.String: Value += value.Value; break;
					case VariableValueType.Integer: Value = (long.Parse(Value) + long.Parse(value.Value)).ToString(); break;
					case VariableValueType.Float: Value = (decimal.Parse(Value) + decimal.Parse(value.Value)).ToString(); break;

					default: throw new SectionException("", $"({this}) + ({value})", "", "Not supported");
				}
			}
			
			throw new NotImplementedException($"({this}) + ({value})");
		}

		internal void Subtract(VariableValue value)
		{
			if (IsLiteral && value.IsLiteral)
			{
				switch (Type)
				{
					// case VariableValueType.String: Value += value.Value; break; TODO:: Implement string subtraction? Figure out what that would even be
					case VariableValueType.Integer: Value = (long.Parse(Value) - long.Parse(value.Value)).ToString(); break;
					case VariableValueType.Float: Value = (decimal.Parse(Value) - decimal.Parse(value.Value)).ToString(); break;

					default: throw new SectionException("", $"({this}) - ({value})", "", "Not supported");
				}
			}


			throw new NotImplementedException($"({this}) - ({value})");
		}

		internal void Multiply(VariableValue value)
		{
			if (IsLiteral && value.IsLiteral)
			{
				switch (Type)
				{
					// case VariableValueType.String: Value += value.Value; break; TODO:: Implement string multiplication, as long as its between a string and a number
					case VariableValueType.Integer: Value = (long.Parse(Value) * long.Parse(value.Value)).ToString(); break;
					case VariableValueType.Float: Value = (decimal.Parse(Value) * decimal.Parse(value.Value)).ToString(); break;

					default: throw new SectionException("", $"({this}) * ({value})", "", "Not supported");
				}
			}


			throw new NotImplementedException($"({this}) * ({value})");
		}

		internal void Divide(VariableValue value)
		{
			if (IsLiteral && value.IsLiteral)
			{
				switch (Type)
				{
					// case VariableValueType.String: Value += value.Value; break; TODO:: Implement string division, as long as its between a string and a number
					case VariableValueType.Integer: Value = (long.Parse(Value) / long.Parse(value.Value)).ToString(); break;
					case VariableValueType.Float: Value = (decimal.Parse(Value) / decimal.Parse(value.Value)).ToString(); break;

					default: throw new SectionException("", $"({this}) / ({value})", "", "Not supported");
				}
			}


			throw new NotImplementedException($"({this}) / ({value})");
		}

		internal void CopyFrom(VariableValue value)
		{
			Value = value.Value;
			Left = value.Left;
			Right = value.Right;
			Type = value.Type;
			Operator = value.Operator;
			IsLiteral = value.IsLiteral;
		}
		#endregion

		void RecursivelyResolveLeftRight(VariablesCache parentVariables, List<string> values, out VariableValue left, out Operator leftRightOperator, out VariableValue right)
		{
			if (values.Count <= 1)
			{
				throw new NotImplementedException("This really should not be possible, fixme if we somehow end up here");
			}
			else if (values.Count == 2)
			{
				throw new NotImplementedException("");
			}
			else if(values.Count == 3)
			{
				left = new VariableValue(parentVariables, values[0]);
				right = new VariableValue(parentVariables, values[2]);
				if(!TryParseOperator(values[1], out leftRightOperator))
					throw new SectionException(values[0] + ' ', values[1], ' ' + values[2], "Unknown operator, expected +, -, *, or /");
				else if (leftRightOperator == Operator.Divide && ((right.Type == VariableValueType.Integer && int.Parse(right.Value) == 0) || (right.Type == VariableValueType.Float && float.Parse(right.Value) == 0f)))
					throw new SectionException(values[0] + ' ' + values[1], values[2], "", "Division by zero");
			}
			else
			{
				if(values[0] == "(")
				{
					var leftValue = GetParenthesesGroup(values, 0, out var endsAt);
					if(endsAt < values.Count - 1)
					{
						var leftValueString = string.Join(" ", leftValue);
						left = new VariableValue(parentVariables, leftValueString);
						if (!TryParseOperator(values[endsAt+1], out leftRightOperator))
							throw new SectionException(leftValueString + ' ', values[endsAt + 1], ' ' + string.Join(" ", values.Skip(endsAt + 2)), "Unknown operator, expected +, -, *, or /");
						right = new VariableValue(parentVariables, string.Join(" ", values.Skip(endsAt + 2)));
					}
					// Only this group, parse as is
					else
					{
						if (leftValue.Count == 3)
						{
							RecursivelyResolveLeftRight(parentVariables, leftValue, out left, out leftRightOperator, out right);
						}
						else
						{
							left = new VariableValue(parentVariables, string.Join(" ", leftValue));
							leftRightOperator = Operator.Undefined;
							right = null;
						}
					}
				}
				else
				{
					left = new VariableValue(parentVariables, values[0]);
					if (!TryParseOperator(values[1], out leftRightOperator))
						throw new SectionException(values[0] + ' ', values[1], ' ' + string.Join(" ", values.Skip(2)), "Unknown operator, expected +, -, *, or /");
					right = new VariableValue(parentVariables, string.Join(" ", values.Skip(2)));
				}
			}
		}

		static bool TryParseOperator(string raw, out Operator outOperator)
		{
			switch (raw)
			{
				case "+": outOperator = Operator.Add;      return true;
				case "-": outOperator = Operator.Subtract; return true;
				case "*": outOperator = Operator.Multiply; return true;
				case "/": outOperator = Operator.Divide;   return true;
				default: outOperator = Operator.Undefined; return false;
			}
		}

		static List<string> GetParenthesesGroup(List<string> values, int startAt, out int endAt)
		{
			if (values[startAt] == "(")
			{
				var numberOfBrackets = 1;
				for (endAt = startAt + 1; endAt < values.Count; endAt++)
				{
					if (values[endAt] == "(")
						numberOfBrackets++;
					else if (values[endAt] == ")")
						numberOfBrackets--;

					if (numberOfBrackets == 0)
					{
						return values.Skip(startAt + 1).Take(endAt - startAt - 1).ToList();
					}
				}
			}

			endAt = startAt;
			return null;
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
