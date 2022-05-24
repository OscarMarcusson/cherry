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

		[TestMethod]
		public void ThrowsExceptionForMissingIn()
		{
			Assert.ThrowsException<SectionException>(() => new Foreach("foreach i something-incorrect-here range:1-10"));
		}

		[TestMethod]
		public void CanGenerateRange()
		{
			var loop = new Foreach("foreach i in range:1-10");
			Assert.AreEqual(ForeachResourceType.Range, loop.ResourceType);
			Assert.IsNotNull(loop.Values);
			Assert.AreEqual(10, loop.Values.Length);

			// Double check that the range is valid by sampling each value
			for (int i = 1; i <= 10; i++)
				Assert.AreEqual(i, loop.Values[i - 1]);
		}

		[TestMethod]
		public void CanGenerateFiles()
		{
			var loop = new Foreach("foreach i in file:*.txt");
			Assert.AreEqual(ForeachResourceType.File, loop.ResourceType);
			Assert.IsNotNull(loop.Values);
		}
	}
}
