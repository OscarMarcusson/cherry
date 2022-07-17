using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cherry;

namespace Compilation
{
	[TestClass]
	public class CompileTests
	{
		[TestMethod]
		public void TestNullInput()
		{
			var compiler = new Compiler(new CompilerArguments(), throwExceptionOnFail:true);
			Assert.ThrowsException<Exception>(() => compiler.Compile());
		}

		[TestMethod]
		public void TestEmptyInput()
		{
			var compiler = new Compiler(new CompilerArguments { Input = "" }, throwExceptionOnFail: true);
			Assert.ThrowsException<Exception>(() => compiler.Compile());
		}

		[TestMethod]
		public void TestFileNotFoundInput()
		{
			var compiler = new Compiler(new CompilerArguments { Input = "DOES_NOT_EXIST_SJD7a9dhasbd sa9d7nas 97sand9n02q0dnq80n0s<nd aD as08dn20q8n 0qndnsadn 8AS(Dn 0q28n d(=Adnas=D(nsa d80asnD==" }, throwExceptionOnFail: true);
			Assert.ThrowsException<Exception>(() => compiler.Compile());
		}
	}
}
