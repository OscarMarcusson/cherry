using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cherry;
using Cherry.Intermediate;
using Cherry.Internal.Reader;

namespace Functions
{
	[TestClass]
	public class Returns
	{
		[TestMethod]
		public void FailsIfStartingWithWrongKeyword()
		{
			var variables = new VariablesCache();
			Assert.ThrowsException<SectionException>(() => new Return(variables, new LineReader("error 123")));
		}

		[TestMethod]
		public void VoidReturn()
		{
			var variables = new VariablesCache();
			var r = new Return(variables, new LineReader("return"));
			Assert.AreEqual(false, r.HasValue);
			Assert.IsNull(r.Value);
		}

		[TestMethod]
		public void ReturnValue()
		{
			var variables = new VariablesCache();
			var r = new Return(variables, new LineReader("return 1"));
			Assert.AreEqual(true, r.HasValue);
			Assert.AreEqual("1", r.Value.Value);
		}

		[TestMethod]
		public void ReturnFormula()
		{
			var variables = new VariablesCache();
			var r = new Return(variables, new LineReader("return 1 + 5 * 10"));
			Assert.AreEqual(true, r.HasValue);
			Assert.AreEqual("51", r.Value.Value);
		}
	}
}
