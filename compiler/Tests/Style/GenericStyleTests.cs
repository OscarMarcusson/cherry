using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cherry.Intermediate;
using Cherry.Internal.Reader;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Style
{
	[TestClass]
	public class GenericStyleTests
	{
		[TestMethod]
		public void CanCreateStyle()
		{
			var documentReader = DocumentReader.Raw
				(
					"style",
					"	text",
					"		color = red"
				);

			var document = new Document(documentReader, Cherry.CompilerArguments.Test);
			Assert.IsNotNull(document.Style);
			Assert.IsNotNull(document.Style.Elements);
			Assert.AreEqual(1, document.Style.Elements.Count);
			Assert.IsTrue(document.Style.Elements.ContainsKey("text"));
			Assert.IsNotNull(document.Style.Elements["text"].Values);
			Assert.IsTrue(document.Style.Elements["text"].Values.ContainsKey("color"));
			Assert.AreEqual("red", document.Style.Elements["text"].Values["color"]);
		}

		[TestMethod]
		public void Global()
		{
			var style = new Cherry.Intermediate.Style(LineReader.ParseLineWithChildren("style"));
			Assert.IsTrue(style.IsGlobal);
		}

		[TestMethod]
		public void CustomName()
		{
			var style = new Cherry.Intermediate.Style(LineReader.ParseLineWithChildren("style test"));
			Assert.IsFalse(style.IsGlobal);
			Assert.AreEqual("test", style.Name);
		}
	}
}
