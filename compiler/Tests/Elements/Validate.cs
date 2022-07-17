using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cherry;
using Cherry.Intermediate;
using Cherry.Internal;
using Cherry.Internal.Reader;
using Cherry.Intermediate.Elements;

namespace Elements
{
	[TestClass]
	public class Validate
	{
		[TestMethod]
		public void ElementNotFound()
		{
			Assert.ThrowsException<SectionException>(() => ElementParser.ToElement(reader: LineReader.ParseLineWithChildren("test123"), parent: null, CompilerArguments.Test));
		}

		[TestMethod]
		public void Text()
		{
			var element = ElementParser.ToElement(reader: new LineReader("text"), parent: null);
			Assert.IsInstanceOfType(element, typeof(TextElement));
		}

		[TestMethod]
		public void Title()
		{
			var element = ElementParser.ToElement(reader: new LineReader("title = \"Hello World\""), parent: null, CompilerArguments.Test);
			Assert.IsInstanceOfType(element, typeof(TitleElement));
			Assert.AreEqual(1, ((TitleElement)element).Heading);
		}

		[TestMethod]
		public void CustomTitle()
		{
			var element = ElementParser.ToElement(reader: new LineReader("title_1 = \"Hello World\""), parent: null, CompilerArguments.Test);
			Assert.IsInstanceOfType(element, typeof(TitleElement));
			Assert.AreEqual(1, ((TitleElement)element).Heading);

			element = ElementParser.ToElement(reader: new LineReader("title_2 = \"Hello World\""), parent: null, CompilerArguments.Test);
			Assert.IsInstanceOfType(element, typeof(TitleElement));
			Assert.AreEqual(2, ((TitleElement)element).Heading);

			element = ElementParser.ToElement(reader: new LineReader("title_3 = \"Hello World\""), parent: null, CompilerArguments.Test);
			Assert.IsInstanceOfType(element, typeof(TitleElement));
			Assert.AreEqual(3, ((TitleElement)element).Heading);

			element = ElementParser.ToElement(reader: new LineReader("title_4 = \"Hello World\""), parent: null, CompilerArguments.Test);
			Assert.IsInstanceOfType(element, typeof(TitleElement));
			Assert.AreEqual(4, ((TitleElement)element).Heading);

			element = ElementParser.ToElement(reader: new LineReader("title_5 = \"Hello World\""), parent: null, CompilerArguments.Test);
			Assert.IsInstanceOfType(element, typeof(TitleElement));
			Assert.AreEqual(5, ((TitleElement)element).Heading);

			element = ElementParser.ToElement(reader: new LineReader("title_6 = \"Hello World\""), parent: null, CompilerArguments.Test);
			Assert.IsInstanceOfType(element, typeof(TitleElement));
			Assert.AreEqual(6, ((TitleElement)element).Heading);
		}

		[TestMethod]
		public void Image()
		{
			var element = ElementParser.ToElement(reader: new LineReader("image"), parent: null, CompilerArguments.Test);
			Assert.IsInstanceOfType(element, typeof(ImageElement));
		}

		[TestMethod]
		public void Area()
		{
			var element = ElementParser.ToElement(reader: new LineReader("area"), parent: null, CompilerArguments.Test);
			Assert.IsInstanceOfType(element, typeof(AreaElement));
		}

		[TestMethod]
		public void Link()
		{
			var element = ElementParser.ToElement(reader: new LineReader("link"), parent: null, CompilerArguments.Test);
			Assert.IsInstanceOfType(element, typeof(LinkElement));
		}

		[TestMethod]
		public void IFrame()
		{
			var element = ElementParser.ToElement(reader: new LineReader("iframe"), parent: null, CompilerArguments.Test);
			Assert.IsInstanceOfType(element, typeof(IframeElement));
		}

		[TestMethod]
		public void Include()
		{
			var element = ElementParser.ToElement(reader: new LineReader("include = \"test.md\""), parent: null, CompilerArguments.Test);
			Assert.IsInstanceOfType(element, typeof(IncludeElement));

			Assert.ThrowsException<SectionException>(() => ElementParser.ToElement(reader: new LineReader("include = \"test.not_supported_file_type\""), parent: null, CompilerArguments.Test));
		}
	}
}
