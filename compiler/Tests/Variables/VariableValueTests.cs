using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimplifiedUserInterfaceFramework.Intermediate;

namespace Variables
{
	[TestClass]
	public class VariableValueTests
	{
		[TestMethod]
		public void EmptyInputMapsToEmptyType()
		{
			Assert.AreEqual(VariableValueType.Empty, new VariableValue("").Type);
			Assert.AreEqual(VariableValueType.Empty, new VariableValue("       ").Type);
		}


		[TestMethod]
		public void CanParseIntegerLiteral()
		{
			Assert.AreEqual(VariableValueType.Integer, new VariableValue("1").Type);
			Assert.AreEqual(VariableValueType.Integer, new VariableValue("532").Type);
			Assert.AreEqual(VariableValueType.Integer, new VariableValue("87654").Type);

			Assert.AreEqual(123, int.Parse(new VariableValue("123").Value));
			Assert.AreEqual(86345, int.Parse(new VariableValue("86345").Value));
		}


		[TestMethod]
		public void CanParseFloatLiteral()
		{
			Assert.AreEqual(VariableValueType.Float, new VariableValue("1.0").Type);
			Assert.AreEqual(VariableValueType.Float, new VariableValue("0.13532").Type);
			Assert.AreEqual(VariableValueType.Float, new VariableValue("8723.23523").Type);

			Assert.AreEqual(0.2, double.Parse(new VariableValue("0.2").Value));
			Assert.AreEqual(1572.32324, double.Parse(new VariableValue("1572.32324").Value));
		}


		[TestMethod]
		public void CanParseStringLiteral()
		{
			Assert.AreEqual(VariableValueType.String, new VariableValue("\"abc123\"").Type);
			Assert.AreEqual(VariableValueType.String, new VariableValue("\"\"").Type);
			Assert.AreEqual(VariableValueType.String, new VariableValue("\"1\"").Type);

			Assert.AreEqual("abc123", new VariableValue("\"abc123\"").Value);
			Assert.AreEqual("1", new VariableValue("\"1\"").Value);
			Assert.AreEqual("", new VariableValue("\"\"").Value);
		}


		[TestMethod]
		public void CanParseBoolLiteral()
		{
			Assert.AreEqual(VariableValueType.Bool, new VariableValue("true").Type);
			Assert.AreEqual(VariableValueType.Bool, new VariableValue("false").Type);
			// Make sure that we ignore true / false that are not lowercase
			Assert.AreNotEqual(VariableValueType.Bool, new VariableValue("TRUE").Type);
			Assert.AreNotEqual(VariableValueType.Bool, new VariableValue("FALSE").Type);

			Assert.AreEqual(true, bool.Parse(new VariableValue("true").Value));
			Assert.AreEqual(false, bool.Parse(new VariableValue("false").Value));
		}


		[TestMethod]
		public void CanParseVariableReference()
		{
			Assert.AreEqual(VariableValueType.Reference, new VariableValue("a").Type);
			Assert.AreEqual(VariableValueType.Reference, new VariableValue("some-value").Type);

			Assert.AreEqual("a", new VariableValue("a").Value);
			Assert.AreEqual("some-value", new VariableValue("some-value").Value);
		}
	}
}
