using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cherry;
using Cherry.Intermediate;
using Cherry.Internal.Reader;

namespace Functions
{
	[TestClass]
	public class FunctionCalls
	{
		[TestMethod]
		public void FailsIfMissingStartParentheses()
		{
			var variables = new VariablesCache();
			Assert.ThrowsException<SectionException>(() => new FunctionCall(variables, "print"));
		}

		[TestMethod]
		public void FailsIfMissingEndParentheses()
		{
			var variables = new VariablesCache();
			Assert.ThrowsException<SectionException>(() => new FunctionCall(variables, "print("));
		}

		[TestMethod]
		public void FailsIfIncorrectParenthesesOrder()
		{
			var variables = new VariablesCache();
			Assert.ThrowsException<SectionException>(() => new FunctionCall(variables, "print)("));
		}

		[TestMethod]
		public void FailsIfMissingName()
		{
			var variables = new VariablesCache();
			Assert.ThrowsException<SectionException>(() => new FunctionCall(variables, "()"));
		}

		[TestMethod]
		public void CanParseWithNoArguments()
		{
			var variables = new VariablesCache();
			var f = new FunctionCall(variables, "print()");
			Assert.IsNull(f.Arguments);
		}

		[TestMethod]
		public void CanParseWithArguments()
		{
			var variables = new VariablesCache();
			var f = new FunctionCall(variables, "print(\"Hello\")");
			Assert.IsNotNull(f.Arguments);
			Assert.AreEqual(1, f.Arguments.Length);
			Assert.AreEqual(VariableValueType.String, f.Arguments[0].Type);
			Assert.AreEqual("Hello", f.Arguments[0].Value);

			f = new FunctionCall(variables, "max(1, 5)");
			Assert.IsNotNull(f.Arguments);
			Assert.AreEqual(2, f.Arguments.Length);
			Assert.AreEqual(VariableValueType.Integer, f.Arguments[0].Type);
			Assert.AreEqual("1", f.Arguments[0].Value);   
			Assert.AreEqual(VariableValueType.Integer, f.Arguments[1].Type);
			Assert.AreEqual("5", f.Arguments[1].Value);
		}
	}
}
