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
		Reference,
	}

	public class VariableValue
	{
		public readonly VariableValueType Type;
		public readonly Dictionary<string, Variable> ReferencedVariables;
		public readonly string Value;


		public VariableValue(string raw)
		{
			if (string.IsNullOrWhiteSpace(raw))
			{
				Type = VariableValueType.Empty;
				return;
			}

			raw = raw.Trim();
			if (raw.StartsWith('"') && raw.EndsWith('"'))
			{
				Type = VariableValueType.String;
				raw = raw.Length > 2
						? raw.Substring(1, raw.Length - 2)
						: ""
						;
				Value = raw;
			}
			else if(raw.All(x => char.IsDigit(x)))
			{
				Type = VariableValueType.Integer;
				Value = raw;
			}
			else if (raw.All(x => char.IsDigit(x) || x == '.'))
			{
				Type = VariableValueType.Float;
				Value = raw.StartsWith('.')
							? "0" + raw
							: raw.EndsWith('.')
								? raw.Trim('.')
								: raw
							;
			}
		}
	}
}
