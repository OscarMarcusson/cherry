using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimplifiedUserInterfaceFramework;
using SimplifiedUserInterfaceFramework.Internal;

namespace Parser
{
	[TestClass]
	public class ArgumentTests
	{
		const string Path = "input.txt";

		const string ShortKey         = "r";
		const string ShortKeyArgument = "-" + ShortKey;
		const string LongKey          = "real-time";
		const string LongKeyArgument  = "--" + LongKey;

		const string ShortStringKey         = "o";
		const string ShortStringKeyArgument = "-" + ShortStringKey;
		const string LongStringKey          = "output";
		const string LongStringKeyArgument  = "--" + LongStringKey;

		const string StringValue = "output.html";


		ArgumentReader GetReader(params string[] input) => new ArgumentReader(input, exitOnFatal: false);


		[TestMethod]
		public void TestEmpty()
		{
			var reader = GetReader();
			Assert.IsNull(reader.Last());
			TestNotFound(reader);
		}


		[TestMethod]
		public void TestInputOnly()
		{
			var reader = GetReader(Path);
			Assert.AreEqual(reader.Last(), Path);
			TestNotFound(reader);
		}


		[TestMethod]
		public void TestShortKey()
		{
			var reader = GetReader(ShortKeyArgument, Path);
			Assert.AreEqual(reader.Last(), Path);
			Assert.IsFalse(reader.Exists(ShortKey));
			Assert.IsTrue(reader.Exists(ShortKey, LongKey));
			TestNotFound(reader);
		}


		[TestMethod]
		public void TestLongKey()
		{
			var reader = GetReader(LongKeyArgument, Path);
			Assert.AreEqual(reader.Last(), Path);
			Assert.IsTrue(reader.Exists(LongKey));
			Assert.IsTrue(reader.Exists(ShortKey, LongKey));
			TestNotFound(reader);
		}


		[TestMethod]
		public void TestShortStringException()
		{
			var reader = GetReader(ShortStringKeyArgument, Path);
			Assert.AreEqual(reader.Last(), Path);
			Assert.IsNull(reader.String(LongStringKey));
			Assert.ThrowsException<Exception>(() => reader.String(ShortStringKey, LongStringKey));
			TestNotFound(reader);
		}

		[TestMethod]
		public void TestLongStringException()
		{
			var reader = GetReader(LongStringKeyArgument, Path);
			Assert.AreEqual(reader.Last(), Path);
			Assert.ThrowsException<Exception>(() => reader.String(LongStringKey));
			Assert.ThrowsException<Exception>(() => reader.String(ShortStringKey, LongStringKey));
			TestNotFound(reader);
		}


		[TestMethod]
		public void TestShortString()
		{
			var reader = GetReader(ShortStringKeyArgument, StringValue, Path);
			Assert.AreEqual(reader.Last(), Path);
			Assert.IsNull(reader.String(LongStringKey));
			Assert.AreEqual(StringValue, reader.String(ShortStringKey, LongStringKey));
			TestNotFound(reader);
		}

		[TestMethod]
		public void TestLongString()
		{
			var reader = GetReader(LongStringKeyArgument, StringValue, Path);
			Assert.AreEqual(reader.Last(), Path);
			Assert.AreEqual(StringValue, reader.String(LongStringKey));
			Assert.AreEqual(StringValue, reader.String(ShortStringKey, LongStringKey));
			TestNotFound(reader);
		}


		void TestNotFound(ArgumentReader reader)
		{
			Assert.IsNull(reader.String("a"));
			Assert.IsNull(reader.String("a", "b"));
			Assert.AreEqual(reader.Enum<LogLevel>("a"), default);
			Assert.AreEqual(reader.Enum<LogLevel>("a", "b"), default);
		}
	}
}
