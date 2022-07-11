using System;
using System.Collections.Generic;
using System.Text;
using SimplifiedUserInterfaceFramework.Intermediate;

namespace SimplifiedUserInterfaceFramework.Utilities
{
	public static class Keywords
	{

		public static bool IsKeyword(string key) 
			=> IsControlFlow(key) 
			|| IsDeclaration(key) 
			|| IsOperator(key)
			|| IsNull(key)
			;



		public static bool IsNull(string key) => key == "null";


		public static bool IsControlFlow(string key)
		{
			switch(key)
			{
				case "if":
				case "else":
				case "for":
				case "while":
					return true;

				default:
					return false;
			}
		}


		public static bool IsDeclaration(string key)
		{
			switch (key)
			{
				case Variable.DynamicAccessType:
				case Variable.ReadOnlyAccessType:
				case "def":
				case "data":
				case "new":
					return true;

				default:
					return false;
			}
		}


		public static bool IsOperator(string key)
		{
			switch (key)
			{
				case "+":
				case "+=":
				case "++":

				case "-":
				case "-=":
				case "--":

				case "/":
				case "/=":

				case "*":
				case "*=":

				case "&&":
				case "&=":

				case "|=":
				case "||":
					return true;

				default:
					return false;
			}
		}
	}
}
