using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimplifiedUserInterfaceFramework.Internal.Reader;

namespace Readers
{
	[TestClass]
	public class LineReaderTests
	{
		[TestMethod]
		public void CanParseSingleLine()
		{
			var word = "Hello";
			var reader = new LineReader(word);

			Assert.AreEqual(0, reader.Indentation);
			Assert.AreEqual(word, reader.Text);
		}


		[TestMethod]
		public void Indentation()
		{
			var word = "Indent test";

			var reader = new LineReader(word);
			Assert.AreEqual(0, reader.Indentation);
			Assert.AreEqual(word, reader.Text);

			reader = new LineReader("\t\t\t" + word);
			Assert.AreEqual(3, reader.Indentation);
			Assert.AreEqual(word, reader.Text);
		}


		[TestMethod]
		public void MultipleLineIndentations()
		{
			var code = "line 1\n\tchild line to line 1\n\t\tchild to child\n\t\tchild to child 2\nline 2\nline3";
			var readers = LineReader.ParseLines(code);

			Assert.AreEqual(3, readers.Length);
			Assert.AreEqual(1, readers[0].Children.Count);
			Assert.AreEqual(2, readers[0].Children[0].Children.Count);
			Assert.AreEqual(0, readers[1].Children.Count);
			Assert.AreEqual(0, readers[2].Children.Count);
		}
	}
}
