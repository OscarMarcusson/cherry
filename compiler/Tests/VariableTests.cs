using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimplifiedUserInterfaceFramework;
using SimplifiedUserInterfaceFramework.Intermediate;

namespace UnitTests
{
	[TestClass]
	public class VariableTests
	{



		[TestMethod]
		public void TestAccessabilityType()
		{
			Assert.ThrowsException<SectionException>(() => new Variable("this can't be parsed as a variable"));

			Assert.AreEqual(new Variable("var a").AccessType, VariableType.Dynamic);
			Assert.AreEqual(new Variable("let a").AccessType, VariableType.ReadOnly);
			Assert.AreEqual(new Variable("var a = 5").AccessType, VariableType.Dynamic);
			Assert.AreEqual(new Variable("let a = 5").AccessType, VariableType.ReadOnly);

			Assert.AreEqual(new Variable("var int a").AccessType, VariableType.Dynamic);
			Assert.AreEqual(new Variable("let int a").AccessType, VariableType.ReadOnly);
			Assert.AreEqual(new Variable("var int a = 5").AccessType, VariableType.Dynamic);
			Assert.AreEqual(new Variable("let int a = 5").AccessType, VariableType.ReadOnly);
		}

		[TestMethod]
		public void TestIncorrectTypes()
		{
			Assert.ThrowsException<SectionException>(() => new Variable("var void name"));
			Assert.ThrowsException<SectionException>(() => new Variable("var if name"));
			Assert.ThrowsException<SectionException>(() => new Variable("var else name"));
			Assert.ThrowsException<SectionException>(() => new Variable("var for name"));
			Assert.ThrowsException<SectionException>(() => new Variable("var while name"));
			Assert.ThrowsException<SectionException>(() => new Variable("var var name"));
			Assert.ThrowsException<SectionException>(() => new Variable("var let name"));
			Assert.ThrowsException<SectionException>(() => new Variable("var data name"));
			Assert.ThrowsException<SectionException>(() => new Variable("var def name"));
		}
	}
}
