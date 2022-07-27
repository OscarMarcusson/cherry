using System;
using System.Collections.Generic;
using System.Text;

namespace Cherry.Utilities
{
	public static class TypeTranslator
	{
		public static string ToCpp(string type)
		{
			switch (type)
			{
				// Integers
				case "i8":
				case "i16":
					return "short int";

				case "i32":
					return "int";

				case "int": // <<<< Default
				case "i64":
					return "long int";

				case "i128":
					return "long long int";


				// Unsigned integers
				case "u8":
				case "u16":
					return "unsigned short int";

				case "u32":
					return "unsigned int";

				case "uint": // <<<< Default
				case "u64":
					return "unsigned long int";

				case "u128":
					return "unsigned long long int";


				// Floats
				case "f32":
					return "float";

				case "f64":
				case "float":  // <<<< Default
					return "double";

				case "f128":
					return "long double";


				// Other builtin types
				case "bool":
					return "bool";

				case "string":
					return "std::string";


				default: return type;
			}
		}
	}
}
