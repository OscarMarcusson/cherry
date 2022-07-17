using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cherry.Intermediate;
using Cherry.Internal.Reader;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RootKeywords
{
	[TestClass]
	public class WindowTests
	{
		[TestMethod]
		public void CanCreateWindow()
		{
			var documentReader = DocumentReader.Raw
				(
					"window", 
					"	title = \"Hello World!\""
				);

			var document = new Document(documentReader, Cherry.CompilerArguments.Test);
			Assert.IsNotNull(document.MainWindow);
			Assert.IsNotNull(document.MainWindow.Children);
			Assert.AreEqual(1, document.MainWindow.Children.Count);
		}

		// TODO:: When we add "window [NAME]" syntax we have to add tests for that as well
	}
}
