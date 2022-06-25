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
			var variables = new VariablesCache();
			var a = new Variable(variables, VariableType.Dynamic, "a", "5");

			Assert.AreEqual("true", new IfStatment(variables, "if true").Condition.Value);
			Assert.AreEqual(IfElseType.If, new IfStatment(variables, "if a > 5").Condition.Operator);
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
			var variables = new VariablesCache();
			var a = new Variable(variables, VariableType.ReadOnly, "a", "5");

			Assert.AreEqual("true", new IfStatment(variables, "if true").Condition.Value);
			Assert.AreEqual("true", new IfStatment(variables, "if 1 == 1").Condition.Value);
			Assert.AreEqual("true", new IfStatment(variables, "if a == 5").Condition.Value);
			Assert.AreEqual("true", new IfStatment(variables, "if 1 > 0").Condition.Value);
			Assert.AreEqual("true", new IfStatment(variables, "if a > 0").Condition.Value);
			Assert.AreEqual("true", new IfStatment(variables, "if 1 >= 1").Condition.Value);
			Assert.AreEqual("true", new IfStatment(variables, "if a >= 5").Condition.Value);
			Assert.AreEqual("false", new IfStatment(variables, "if 1 < 0").Condition.Value);
			Assert.AreEqual("false", new IfStatment(variables, "if a < 0").Condition.Value);
			Assert.AreEqual("true", new IfStatment(variables, "if 1 <= 1").Condition.Value);
			Assert.AreEqual("true", new IfStatment(variables, "if a <= 5").Condition.Value);
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
			var variables = new VariablesCache();
			Assert.ThrowsException<SectionException>(() => new IfStatment(variables, "if 5"));
			Assert.ThrowsException<SectionException>(() => new IfStatment(variables, "if \"yes\""));
			Assert.ThrowsException<SectionException>(() => new IfStatment(variables, "else if 5"));
			Assert.ThrowsException<SectionException>(() => new IfStatment(variables, "else if \"yes\""));
		}
	}
}
