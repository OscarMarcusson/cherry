using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cherry.Intermediate;
using Cherry.Internal.Reader;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Window
{
	[TestClass]
	public class CreateWindow
	{
		[TestMethod]
		public void MainWindow()
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
