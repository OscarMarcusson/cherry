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


		[TestMethod]
		public void CanParse2VariableMath()
		{
			Assert.AreEqual(VariableValueType.Integer, new VariableValue("1 + 2").Type);
			Assert.AreEqual(VariableValueType.Float, new VariableValue("1.5 + 3").Type);

			Assert.AreEqual(VariableValueType.Integer, new VariableValue("3 - 1").Type);
			Assert.AreEqual(VariableValueType.Float, new VariableValue("5 - 0.1").Type);

			Assert.AreEqual(VariableValueType.Integer, new VariableValue("5 * 82").Type);
			Assert.AreEqual(VariableValueType.Float, new VariableValue("0.63 * 99").Type);

			Assert.AreEqual(VariableValueType.Integer, new VariableValue("10 / 5").Type);
			Assert.AreEqual(VariableValueType.Float, new VariableValue("13 / 7.0").Type);


			Assert.AreEqual(1, int.Parse(new VariableValue("-1 + 2").Value));
			Assert.AreEqual(2.5m, decimal.Parse(new VariableValue("0.5 + 2").Value));

			Assert.AreEqual(5, int.Parse(new VariableValue("7 - 2").Value));
			Assert.AreEqual(0.75m, decimal.Parse(new VariableValue("3 - 2.25").Value));

			Assert.AreEqual(10, int.Parse(new VariableValue("5 * 2").Value));
			Assert.AreEqual(5.2m, decimal.Parse(new VariableValue("1.3 * 4").Value));

			Assert.AreEqual(5, int.Parse(new VariableValue("20 / 4").Value));
			Assert.AreEqual(0.65m, decimal.Parse(new VariableValue("5.2 / 8").Value));
		}


		[TestMethod]
		public void CanConcatinate2ValuesToString()
		{
			Assert.AreEqual(VariableValueType.String, new VariableValue("\"abc\" + \"123\"").Type);
			Assert.AreEqual(VariableValueType.String, new VariableValue("\"\" + 123").Type);
			Assert.AreEqual(VariableValueType.String, new VariableValue("123 + \"\"").Type);
			Assert.AreEqual(VariableValueType.String, new VariableValue("\"\" + \"\"").Type);

			Assert.AreEqual("", new VariableValue("\"\" + \"\"").Value);
			Assert.AreEqual("abc123", new VariableValue("\"abc\" + 123").Value);
		}


		[TestMethod]
		public void CanDoMultipliedStringConcatinate()
		{
			Assert.AreEqual(VariableValueType.String, new VariableValue("\"abc\" * 5").Type);
			Assert.AreEqual(VariableValueType.String, new VariableValue("\"\" * 5").Type);
			
			Assert.AreEqual("", new VariableValue("\"\" * 5").Value);
			Assert.AreEqual("-----", new VariableValue("\"-\" * 5").Value);
			Assert.AreEqual("abcabcabc", new VariableValue("\"abc\" * 3").Value);

			// Make sure that the order does not matter
			Assert.AreEqual("", new VariableValue("5 * \"\"").Value);
			Assert.AreEqual("-----", new VariableValue("5 * \"-\"").Value);
			Assert.AreEqual("abcabcabc", new VariableValue("3 * \"abc\"").Value);
		}
	}
}
