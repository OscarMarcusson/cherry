using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimplifiedUserInterfaceFramework;
using SimplifiedUserInterfaceFramework.Intermediate.Preprocessor;
using SimplifiedUserInterfaceFramework.Internal;
using SimplifiedUserInterfaceFramework.Internal.Reader;

namespace Preprocessor
{
	[TestClass]
	public class ForeachTests
	{
		[TestMethod]
		public void ThrowsOnEmptyInput ()
		{
			Assert.ThrowsException<SectionException>(() => new Foreach(new LineReader(""), null, null));
			Assert.ThrowsException<SectionException>(() => new Foreach(new LineReader(" \t   \t\t\t  "), null, null));
		}

		[TestMethod]
		public void ThrowsWhenNotStartingWithForeach()
		{
			Assert.ThrowsException<SectionException>(() => new Foreach(new LineReader("not-the-correct-start 132"), null, null));
		}

		[TestMethod]
		public void CanReadTheVariableName()
		{
			var loop = new Foreach(new LineReader("foreach var-name in range:1-10"), null, null);
			Assert.AreEqual("var-name", loop.VariableName);
		}

		[TestMethod]
		public void ThrowsExceptionForMissingIn()
		{
			Assert.ThrowsException<SectionException>(() => new Foreach(new LineReader("foreach i something-incorrect-here range:1-10"), null, null));
		}

		[TestMethod]
		public void ThrowsExceptionForMissingResourceMarker()
		{
			Assert.ThrowsException<SectionException>(() => new Foreach(new LineReader("foreach i in range 1-10"), null, null));
		}

		[TestMethod]
		public void ThrowsExceptionForIncorrectResourceType()
		{
			Assert.ThrowsException<SectionException>(() => new Foreach(new LineReader("foreach i in I'm-completely-wrong:1-10"), null, null));
		}

		[TestMethod]
		public void CanGenerateRange()
		{
			var loop = new Foreach(new LineReader("foreach i in range:1-10"), null, null);
			Assert.AreEqual(ForeachResourceType.Range, loop.ResourceType);
			Assert.IsNotNull(loop.Values);
			Assert.AreEqual(10, loop.Values.Length);

			// Double check that the range is valid by sampling each value
			for (int i = 1; i <= 10; i++)
				Assert.AreEqual(i.ToString(), loop.Values[i - 1]);
		}

		[TestMethod]
		public void WildCardInFilesDirectoryThrowsException()
		{
			Assert.ThrowsException<SectionException>(() => new ForeachFilesTester("foreach i in file:dir1/dir2/dir*/data.txt"));
		}

		[TestMethod]
		public void CanGenerateFiles()
		{
			// TODO:: Implement the compiler level file reading 
			var loop = new ForeachFilesTester("foreach i in file:*.txt");
			Assert.AreEqual(ForeachResourceType.File, loop.ResourceType);
			Assert.IsNotNull(loop.Values);
			Assert.AreEqual(3, loop.Values.Length);
			Assert.AreEqual("test1.txt", loop.Values[0]);
			Assert.AreEqual("test2.txt", loop.Values[1]);
			Assert.AreEqual("test3.txt", loop.Values[2]);
		}

		// Test wrapper to enable easier IO with full control over test cases, which also allows us to skip interfaces or generics later down the road
		class ForeachFilesTester : Foreach
		{
			public ForeachFilesTester(string rawDeclaration) : base(new LineReader(rawDeclaration), null, null) { }
			protected override string[] GetFiles(string directory, string filter) => new[] { "test1.txt", "test2.txt", "test3.txt" };
		}


		[TestMethod]
		public void RangeValueIsAppliedToChildren()
		{
			var line = new LineReader("foreach i in range:1-10");
			line.Children.Add(new LineReader("\tp = i"));

			var loop = new Foreach(line, null, null);
			Assert.AreEqual(10, loop.Children.Count);

			for(int i = 0; i < line.Children.Count; i++)
				Assert.AreEqual((i + 1).ToString(), loop.Children[i].Value);
		}
	}
}
