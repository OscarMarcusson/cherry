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
		public void CanParseSingleIntegerLiteral()
		{
			Assert.AreEqual(VariableValueType.Integer, new VariableValue("1").Type);
			Assert.AreEqual(VariableValueType.Integer, new VariableValue("532").Type);
			Assert.AreEqual(VariableValueType.Integer, new VariableValue("87654").Type);

			Assert.AreEqual(123, int.Parse(new VariableValue("123").Value));
			Assert.AreEqual(86345, int.Parse(new VariableValue("86345").Value));
		}
	}
}
