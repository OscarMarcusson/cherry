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
	}
}
