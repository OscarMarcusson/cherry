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
			var cache = new VariablesCache();
			Assert.AreEqual(VariableValueType.Empty, new VariableValue(cache, "").Type);
			Assert.AreEqual(VariableValueType.Empty, new VariableValue(cache, "       ").Type);
		}


		[TestMethod]
		public void CanParseIntegerLiteral()
		{
			var cache = new VariablesCache();
			Assert.AreEqual(VariableValueType.Integer, new VariableValue(cache, "1").Type);
			Assert.AreEqual(VariableValueType.Integer, new VariableValue(cache, "532").Type);
			Assert.AreEqual(VariableValueType.Integer, new VariableValue(cache, "87654").Type);

			Assert.AreEqual(123, int.Parse(new VariableValue(cache, "123").Value));
			Assert.AreEqual(86345, int.Parse(new VariableValue(cache, "86345").Value));
		}


		[TestMethod]
		public void CanParseFloatLiteral()
		{
			var cache = new VariablesCache();
			Assert.AreEqual(VariableValueType.Float, new VariableValue(cache, "1.0").Type);
			Assert.AreEqual(VariableValueType.Float, new VariableValue(cache, "0.13532").Type);
			Assert.AreEqual(VariableValueType.Float, new VariableValue(cache, "8723.23523").Type);

			Assert.AreEqual(0.2, double.Parse(new VariableValue(cache, "0.2").Value));
			Assert.AreEqual(1572.32324, double.Parse(new VariableValue(cache, "1572.32324").Value));
		}


		[TestMethod]
		public void CanParseStringLiteral()
		{
			var cache = new VariablesCache();
			Assert.AreEqual(VariableValueType.String, new VariableValue(cache, "\"abc123\"").Type);
			Assert.AreEqual(VariableValueType.String, new VariableValue(cache, "\"\"").Type);
			Assert.AreEqual(VariableValueType.String, new VariableValue(cache, "\"1\"").Type);

			Assert.AreEqual("abc123", new VariableValue(cache, "\"abc123\"").Value);
			Assert.AreEqual("1", new VariableValue(cache, "\"1\"").Value);
			Assert.AreEqual("", new VariableValue(cache, "\"\"").Value);
		}


		[TestMethod]
		public void CanResolveLetLiteral()
		{
			var cache = new VariablesCache();
			cache.Create(VariableType.ReadOnly, "a", 5);

			var variableValue = new VariableValue(cache, "1 + a");
			Assert.AreEqual(VariableValueType.Integer, variableValue.Type);
			Assert.AreEqual(6, int.Parse(variableValue.Value));
		}


		[TestMethod]
		public void CanParseBoolLiteral()
		{
			var cache = new VariablesCache();
			Assert.AreEqual(VariableValueType.Bool, new VariableValue(cache, "true").Type);
			Assert.AreEqual(VariableValueType.Bool, new VariableValue(cache, "false").Type);

			Assert.AreEqual(true, bool.Parse(new VariableValue(cache, "true").Value));
			Assert.AreEqual(false, bool.Parse(new VariableValue(cache, "false").Value));
		}


		[TestMethod]
		public void CanParseVariableReference()
		{
			var cache = new VariablesCache();
			cache.Create("var a = 5");
			cache.Create("var some_value = \"some-value\"");

			Assert.AreEqual(VariableValueType.Reference, new VariableValue(cache, "a").Type);
			Assert.AreEqual(VariableValueType.Reference, new VariableValue(cache, "some_value").Type);
		}


		[TestMethod]
		public void CanParse2VariableMath()
		{
			var cache = new VariablesCache();
			Assert.AreEqual(VariableValueType.Integer, new VariableValue(cache, "1 + 2").Type);
			Assert.AreEqual(VariableValueType.Float, new VariableValue(cache, "1.5 + 3").Type);

			Assert.AreEqual(VariableValueType.Integer, new VariableValue(cache, "3 - 1").Type);
			Assert.AreEqual(VariableValueType.Float, new VariableValue(cache, "5 - 0.1").Type);

			Assert.AreEqual(VariableValueType.Integer, new VariableValue(cache, "5 * 82").Type);
			Assert.AreEqual(VariableValueType.Float, new VariableValue(cache, "0.63 * 99").Type);

			Assert.AreEqual(VariableValueType.Integer, new VariableValue(cache, "10 / 5").Type);
			Assert.AreEqual(VariableValueType.Float, new VariableValue(cache, "13 / 7.0").Type);


			Assert.AreEqual(1, int.Parse(new VariableValue(cache, "-1 + 2").Value));
			Assert.AreEqual(2.5m, decimal.Parse(new VariableValue(cache, "0.5 + 2").Value));

			Assert.AreEqual(5, int.Parse(new VariableValue(cache, "7 - 2").Value));
			Assert.AreEqual(0.75m, decimal.Parse(new VariableValue(cache, "3 - 2.25").Value));

			Assert.AreEqual(10, int.Parse(new VariableValue(cache, "5 * 2").Value));
			Assert.AreEqual(5.2m, decimal.Parse(new VariableValue(cache, "1.3 * 4").Value));

			Assert.AreEqual(5, int.Parse(new VariableValue(cache, "20 / 4").Value));
			Assert.AreEqual(0.65m, decimal.Parse(new VariableValue(cache, "5.2 / 8").Value));
		}


		[TestMethod]
		public void CanConcatinate2ValuesToString()
		{
			var cache = new VariablesCache();
			Assert.AreEqual(VariableValueType.String, new VariableValue(cache, "\"abc\" + \"123\"").Type);
			Assert.AreEqual(VariableValueType.String, new VariableValue(cache, "\"\" + 123").Type);
			Assert.AreEqual(VariableValueType.String, new VariableValue(cache, "123 + \"\"").Type);
			Assert.AreEqual(VariableValueType.String, new VariableValue(cache, "\"\" + \"\"").Type);

			Assert.AreEqual("", new VariableValue(cache, "\"\" + \"\"").Value);
			Assert.AreEqual("abc123", new VariableValue(cache, "\"abc\" + 123").Value);
		}


		[TestMethod]
		public void CanDoMultipliedStringConcatinate()
		{
			var cache = new VariablesCache();
			Assert.AreEqual(VariableValueType.String, new VariableValue(cache, "\"abc\" * 5").Type);
			Assert.AreEqual(VariableValueType.String, new VariableValue(cache, "\"\" * 5").Type);
			
			Assert.AreEqual("", new VariableValue(cache, "\"\" * 5").Value);
			Assert.AreEqual("-----", new VariableValue(cache, "\"-\" * 5").Value);
			Assert.AreEqual("abcabcabc", new VariableValue(cache, "\"abc\" * 3").Value);

			// Make sure that the order does not matter
			Assert.AreEqual("", new VariableValue(cache, "5 * \"\"").Value);
			Assert.AreEqual("-----", new VariableValue(cache, "5 * \"-\"").Value);
			Assert.AreEqual("abcabcabc", new VariableValue(cache, "3 * \"abc\"").Value);
		}


		[TestMethod]
		public void CanDoMathWithParentheses()
		{
			var cache = new VariablesCache();
			Assert.AreEqual(VariableValueType.Integer, new VariableValue(cache, "1 + (10 / 2)").Type);
			Assert.AreEqual(VariableValueType.Float, new VariableValue(cache, "(3 * 1.5) / 2").Type);

			Assert.AreEqual(6, int.Parse(new VariableValue(cache, "1 + (10 / 2)").Value));
			Assert.AreEqual(2.25m, decimal.Parse(new VariableValue(cache, "(3 * 1.5) / 2").Value));
		}
	}
}
