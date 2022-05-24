using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimplifiedUserInterfaceFramework;
using SimplifiedUserInterfaceFramework.Intermediate.Preprocessor;
using SimplifiedUserInterfaceFramework.Internal;

namespace Tests
{
	[TestClass]
	public class ForeachTests
	{
		[TestMethod]
		public void ThrowsOnEmptyInput ()
		{
			Assert.ThrowsException<SectionException>(() => new Foreach(null));
			Assert.ThrowsException<SectionException>(() => new Foreach(""));
		}

		[TestMethod]
		public void ThrowsWhenNotStartingWithForeach()
		{
			Assert.ThrowsException<SectionException>(() => new Foreach("not-the-correct-start 132"));
		}

		[TestMethod]
		public void CanReadTheVariableName()
		{
			var loop = new Foreach("foreach var-name in range:1-10");
			Assert.AreEqual("var-name", loop.VariableName);
		}
	}
}
