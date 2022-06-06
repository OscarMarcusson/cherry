using System;
using System.Collections.Generic;
using System.Text;

namespace SimplifiedUserInterfaceFramework.Intermediate
{
	public enum ValueType
	{
		Null,
		String,
		DynamicString,
		Reference
	}

	public class VariableValue
	{
		public readonly ValueType Type;
		public readonly Dictionary<string, Variable> ReferencedVariables;



		public VariableValue(string raw)
		{
			if (string.IsNullOrWhiteSpace(raw))
			{
				Type = ValueType.Null;
				return;
			}

			raw = raw.Trim();
			if (raw.StartsWith('"') && raw.EndsWith('"'))
			{
				Type = ValueType.String;
				raw = raw.Length > 2
						? raw.Substring(1, raw.Length - 2)
						: ""
						;
			}
		}
	}
}
