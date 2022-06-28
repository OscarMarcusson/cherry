using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimplifiedUserInterfaceFramework;
using SimplifiedUserInterfaceFramework.Intermediate;
using SimplifiedUserInterfaceFramework.Internal.Reader;

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
			Assert.AreEqual(Operator.Larger, new IfStatment(variables, "if a > 5").Condition.Operator);
		}

		[TestMethod]
		public void HasCorrectChildren()
		{
			var variables = new VariablesCache();
			var code = LineReader.ParseLineWithChildren("if 1 > 2\n\tvar a = 1\n\tprint(a)");	// TODO:: Rewrite the function call logic to parse this, it currently expects spaces between the name and ( )
			var ifStatement = new IfStatment(variables, code);
			Assert.IsNotNull(ifStatement.Body);
			Assert.AreEqual(2, ifStatement.Body.Length);
			Assert.IsInstanceOfType(ifStatement.Body[0], typeof(Variable));
			Assert.IsInstanceOfType(ifStatement.Body[1], typeof(FunctionCall));
		}

		[TestMethod]
		public void OneLiner()
		{
			var variables = new VariablesCache();
			var ifStatement = new IfStatment(variables, "if 1 > 2; print(\"Hello World\"))");
			Assert.IsNotNull(ifStatement.Body);
			Assert.AreEqual(1, ifStatement.Body.Length);
			Assert.IsInstanceOfType(ifStatement.Body[0], typeof(FunctionCall));
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
