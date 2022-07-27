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
	public class IfElseStatements
	{
		[TestMethod]
		public void DetectsCorrectKey()
		{
			var variables = new VariablesCache();

			Assert.AreEqual(IfElseType.If,     new IfStatment(variables, null, "if true").Type);
			Assert.AreEqual(IfElseType.ElseIf, new IfStatment(variables, null, "else if true").Type);
			Assert.AreEqual(IfElseType.Else,   new IfStatment(variables, null, "else").Type);

			Assert.ThrowsException<SectionException>(() => new IfStatment(variables, null, "IF true"));
			Assert.ThrowsException<SectionException>(() => new IfStatment(variables, null, "ELSE IF true"));
			Assert.ThrowsException<SectionException>(() => new IfStatment(variables, null, "ELSE"));
			Assert.ThrowsException<SectionException>(() => new IfStatment(variables, null, "asdsada"));
			Assert.ThrowsException<SectionException>(() => new IfStatment(variables, null, ""));
		}

		[TestMethod]
		public void ParsesCondition()
		{
			var variables = new VariablesCache();
			var a = new Variable(variables, null, VariableType.Dynamic, "a", "5");

			Assert.AreEqual("true", new IfStatment(variables, null, "if true").Condition.Value);
			Assert.AreEqual(Operator.Larger, new IfStatment(variables, null, "if a > 5").Condition.Operator);
		}

		[TestMethod]
		public void HasCorrectChildren()
		{
			var variables = new VariablesCache();
			var code = LineReader.ParseLineWithChildren("if 1 > 2\n\tvar a = 1\n\tprint(a)");
			var ifStatement = new IfStatment(variables, null, code);
			Assert.IsNotNull(ifStatement.Body);
			Assert.AreEqual(2, ifStatement.Body.Length);
			Assert.IsInstanceOfType(ifStatement.Body[0], typeof(Variable));
			Assert.IsInstanceOfType(ifStatement.Body[1], typeof(FunctionCall));
		}

		[TestMethod]
		public void OneLiner()
		{
			var variables = new VariablesCache();
			var ifStatement = new IfStatment(variables, null, "if 1 > 2; print(\"Hello World\")");
			Assert.IsNotNull(ifStatement.Body);
			Assert.AreEqual(1, ifStatement.Body.Length);
			Assert.IsInstanceOfType(ifStatement.Body[0], typeof(FunctionCall));
		}

		[TestMethod]
		public void CompileTimeResolveLiterals()
		{
			var variables = new VariablesCache();
			var a = new Variable(variables, null, VariableType.ReadOnly, "a", "5");

			Assert.AreEqual("true", new IfStatment(variables, null, "if true").Condition.Value);
			Assert.AreEqual("true", new IfStatment(variables, null, "if 1 == 1").Condition.Value);
			Assert.AreEqual("true", new IfStatment(variables, null, "if a == 5").Condition.Value);
			Assert.AreEqual("true", new IfStatment(variables, null, "if 1 > 0").Condition.Value);
			Assert.AreEqual("true", new IfStatment(variables, null, "if a > 0").Condition.Value);
			Assert.AreEqual("true", new IfStatment(variables, null, "if 1 >= 1").Condition.Value);
			Assert.AreEqual("true", new IfStatment(variables, null, "if a >= 5").Condition.Value);
			Assert.AreEqual("false", new IfStatment(variables, null, "if 1 < 0").Condition.Value);
			Assert.AreEqual("false", new IfStatment(variables, null, "if a < 0").Condition.Value);
			Assert.AreEqual("true", new IfStatment(variables, null, "if 1 <= 1").Condition.Value);
			Assert.AreEqual("true", new IfStatment(variables, null, "if a <= 5").Condition.Value);
		}

		[TestMethod]
		public void ThrowsIfMissingCondition()
		{
			var variables = new VariablesCache();
			Assert.ThrowsException<SectionException>(() => new IfStatment(variables, null, "if"));
			Assert.ThrowsException<SectionException>(() => new IfStatment(variables, null, "if "));
			Assert.ThrowsException<SectionException>(() => new IfStatment(variables, null, "else if"));
			Assert.ThrowsException<SectionException>(() => new IfStatment(variables, null, "else if "));
		}

		[TestMethod]
		public void ThrowsIfElseHasCondition()
		{
			var variables = new VariablesCache();
			Assert.ThrowsException<SectionException>(() => new IfStatment(variables, null, "else true"));
			Assert.ThrowsException<SectionException>(() => new IfStatment(variables, null, "else 5 > 3"));
		}

		[TestMethod]
		public void ThrowsIfConditionIsNotBooleanType()
		{
			var variables = new VariablesCache();
			Assert.ThrowsException<SectionException>(() => new IfStatment(variables, null, "if 5"));
			Assert.ThrowsException<SectionException>(() => new IfStatment(variables, null, "if \"yes\""));
			Assert.ThrowsException<SectionException>(() => new IfStatment(variables, null, "else if 5"));
			Assert.ThrowsException<SectionException>(() => new IfStatment(variables, null, "else if \"yes\""));
		}
	}
}
