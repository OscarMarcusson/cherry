using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Functions
{
	[TestClass]
	public class IfElseStatements
	{
		[TestMethod]
		public void DetectsCorrectKey()
		{
			// TODO::
			// "if"
			// "else if"
			// "else"
			// "askdjasdjsakad" << error
			// uppercase << error
			throw new NotImplementedException();
		}

		[TestMethod]
		public void ParsesCondition()
		{
			// if >>>everything_here<<<
			// same for else if & else
			throw new NotImplementedException();
		}

		[TestMethod]
		public void HasCorrectChildren()
		{
			// if a > 2
			//     print("test")
			throw new NotImplementedException();
		}

		[TestMethod]
		public void OneLiner()
		{
			// if a > 2; print("yes")
			throw new NotImplementedException();
		}

		[TestMethod]
		public void CompileTimeResolveLiterals()
		{
			// will never call print
			// if 1 > 2; print("nope")

			// will be converted to just the print call
			// if true; print("yes")
			throw new NotImplementedException();
		}

		[TestMethod]
		public void ThrowsIfMissingCondition()
		{
			// "if" throws since there is no condition
			throw new NotImplementedException();
		}

		[TestMethod]
		public void ThrowsIfConditionIsNotBooleanType()
		{
			// "if 3" throws, "if true" does not
			throw new NotImplementedException();
		}
	}
}
