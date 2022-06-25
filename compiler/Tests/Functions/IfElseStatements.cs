using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimplifiedUserInterfaceFramework;
using SimplifiedUserInterfaceFramework.Intermediate;

namespace Functions
{
	[TestClass]
	public class IfElseStatements
	{
		[TestMethod]
		public void DetectsCorrectKey()
		{
			var variables = new VariablesCache();

			Assert.AreEqual(IfElseType.If,     new IfStatment(variables, "if true").Type);
			Assert.AreEqual(IfElseType.ElseIf, new IfStatment(variables, "else if true").Type);
			Assert.AreEqual(IfElseType.Else,   new IfStatment(variables, "else").Type);

			Assert.ThrowsException<SectionException>(() => new IfStatment(variables, "IF true"));
			Assert.ThrowsException<SectionException>(() => new IfStatment(variables, "ELSE IF true"));
			Assert.ThrowsException<SectionException>(() => new IfStatment(variables, "ELSE"));
			Assert.ThrowsException<SectionException>(() => new IfStatment(variables, "asdsada"));
			Assert.ThrowsException<SectionException>(() => new IfStatment(variables, ""));
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
			var variables = new VariablesCache();
			Assert.ThrowsException<SectionException>(() => new IfStatment(variables, "if"));
			Assert.ThrowsException<SectionException>(() => new IfStatment(variables, "if "));
			Assert.ThrowsException<SectionException>(() => new IfStatment(variables, "else if"));
			Assert.ThrowsException<SectionException>(() => new IfStatment(variables, "else if "));
		}

		[TestMethod]
		public void ThrowsIfElseHasCondition()
		{
			var variables = new VariablesCache();
			Assert.ThrowsException<SectionException>(() => new IfStatment(variables, "else true"));
			Assert.ThrowsException<SectionException>(() => new IfStatment(variables, "else 5 > 3"));
		}

		[TestMethod]
		public void ThrowsIfConditionIsNotBooleanType()
		{
			// "if 3" throws, "if true" does not
			throw new NotImplementedException();
		}
	}
}
