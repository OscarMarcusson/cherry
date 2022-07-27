using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cherry;
using Cherry.Intermediate;

namespace Parser
{
	[TestClass]
	public class VariableTests
	{



		[TestMethod]
		public void TestAccessabilityType()
		{
			var variables = new VariablesCache();
			Assert.ThrowsException<SectionException>(() => new Variable(variables, null, "this can't be parsed as a variable"));

			Assert.AreEqual(variables.Create("var a").AccessType, VariableType.Dynamic);
			Assert.AreEqual(variables.Create("let b").AccessType, VariableType.ReadOnly);
			Assert.AreEqual(variables.Create("var c = 5").AccessType, VariableType.Dynamic);
			Assert.AreEqual(variables.Create("let d = 5").AccessType, VariableType.ReadOnly);

			Assert.AreEqual(variables.Create("var int e").AccessType, VariableType.Dynamic);
			Assert.AreEqual(variables.Create("let int f").AccessType, VariableType.ReadOnly);
			Assert.AreEqual(variables.Create("var int g = 5").AccessType, VariableType.Dynamic);
			Assert.AreEqual(variables.Create("let int h = 5").AccessType, VariableType.ReadOnly);
		}

		[TestMethod]
		public void TestIncorrectTypes()
		{
			var variables = new VariablesCache();
			Assert.ThrowsException<SectionException>(() => variables.Create("var void name1"));
			Assert.ThrowsException<SectionException>(() => variables.Create("var if name2"));
			Assert.ThrowsException<SectionException>(() => variables.Create("var else name3"));
			Assert.ThrowsException<SectionException>(() => variables.Create("var for name4"));
			Assert.ThrowsException<SectionException>(() => variables.Create("var while name5"));
			Assert.ThrowsException<SectionException>(() => variables.Create("var var name6"));
			Assert.ThrowsException<SectionException>(() => variables.Create("var let name7"));
			Assert.ThrowsException<SectionException>(() => variables.Create("var data name8"));
			Assert.ThrowsException<SectionException>(() => variables.Create("var def name9"));
		}
	}
}
