using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cherry;

namespace Utils
{
	[TestClass]
	public class StringUtilsTest
	{
		[TestMethod]
		public void GetFirstWord()
		{
			var s = "Hello World";
			var index = 0;
			var firstWord = s.GetNextWord(ref index);

			Assert.AreEqual("Hello", firstWord);
		}

		[TestMethod]
		public void IndexIsMovedToStartOfNextWord()
		{
			var s = "Hello   World";
			var index = 0;

			_ = s.GetNextWord(ref index);
			Assert.AreEqual(8, index);
		}

		[TestMethod]
		public void GetRemainderOfWord()
		{
			var s = "Hello World";
			var index = 2;
			var firstWord = s.GetNextWord(ref index);

			Assert.AreEqual("llo", firstWord);

			index = 6;
			var secondWord = s.GetNextWord(ref index);
			Assert.AreEqual("World", secondWord);
		}

		[TestMethod]
		public void GetWords()
		{
			var str = "Hello many words!";
			var words = str.GetWords();

			Assert.AreEqual(3, words.Length);
			Assert.AreEqual("Hello", words[0]);
			Assert.AreEqual("many", words[1]);
			Assert.AreEqual("words!", words[2]);
		}

		[TestMethod]
		public void GetWordsFromIndex()
		{
			var str = "Hello many words!";
			var words = str.GetWords(6);

			Assert.AreEqual(2, words.Length);
			Assert.AreEqual("many", words[0]);
			Assert.AreEqual("words!", words[1]);
		}

		[TestMethod]
		public void SkipsStartingWhitespace()
		{
			var str = "      Hello   World!";
			var words = str.GetWords();
			Assert.AreEqual(2, words.Length);
			Assert.AreEqual("Hello", words[0]);
			Assert.AreEqual("World!", words[1]);
		}

		[TestMethod]
		public void GetWordWithCustomSplit()
		{
			var str = "read.to dot";
			var index = 0;
			var split = new[] { ' ', '.' };
			var first = str.GetNextWord(ref index, split);
			var second = str.GetNextWord(ref index, split);
			var third = str.GetNextWord(ref index, split);

			Assert.AreEqual("read", first);
			Assert.AreEqual("to", second);
			Assert.AreEqual("dot", third);
		}

		[TestMethod]
		public void CanSplitCodeSection()
		{
			var str = "test (bla; (bla bla)); bla";
			var indexOfBreak = str.SplitCodeSection(0, ";", out var code);
			Assert.AreEqual(21, indexOfBreak);
			Assert.AreEqual("test (bla; (bla bla))", code);
		}
	}
}
