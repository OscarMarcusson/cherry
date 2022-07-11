using System;
using System.Collections.Generic;
using System.Text;
using SimplifiedUserInterfaceFramework.Internal.Reader;

namespace SimplifiedUserInterfaceFramework.Intermediate
{
	public class FunctionArgument
	{
		public readonly string Type;
		public readonly string Name;
		public readonly WordReader Value;




		public FunctionArgument(string type, string name, WordReader value = null)
		{
			Type = type;
			Name = name;
			Value = value;
		}
	}
}
