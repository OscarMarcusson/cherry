using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimplifiedUserInterfaceFramework;

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
	}
}
