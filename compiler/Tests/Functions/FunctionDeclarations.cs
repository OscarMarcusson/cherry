using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimplifiedUserInterfaceFramework;
using SimplifiedUserInterfaceFramework.Intermediate;
using SimplifiedUserInterfaceFramework.Internal.Reader;

namespace Functions
{
	[TestClass]
	public class FunctionDeclarations
	{
		[TestMethod]
		public void MissingDefThrowsError()
		{
			var variables = new VariablesCache();
			var code = "void test_function\n\tlet a = 5";
			var reader = LineReader.ParseLineWithChildren(code);

			Assert.ThrowsException<SectionException>(() => new Function(variables, reader));
		}

		[TestMethod]
		public void KeywordsAfterNameThrowsError()
		{
			var variables = new VariablesCache();
			var code = "def void test this will throw error\n\tlet a = 5";
			var reader = LineReader.ParseLineWithChildren(code);

			Assert.ThrowsException<SectionException>(() => new Function(variables, reader));
		}


		[TestMethod]
		public void NoTypeParsedAsVoid()
		{
			var variables = new VariablesCache();
			var code = "def test_function\n\tlet a = 5";
			var reader = LineReader.ParseLineWithChildren(code);

			var function = new Function(variables, reader);
			Assert.AreEqual("void", function.Type);
		}

		[TestMethod]
		public void CanResolveName()
		{
			var variables = new VariablesCache();
			var function1 = new Function(variables, LineReader.ParseLineWithChildren("def test_function_1\n\tlet a = 5"));
			var function2 = new Function(variables, LineReader.ParseLineWithChildren("def void test_function_2\n\tlet a = 5"));
			var function1_args = new Function(variables, LineReader.ParseLineWithChildren("def test_function_1 : string a\n\tlet a = 5"));
			var function2_args = new Function(variables, LineReader.ParseLineWithChildren("def void test_function_2 : string a\n\tlet a = 5"));

			Assert.AreEqual("test_function_1", function1.Name);
			Assert.AreEqual("test_function_2", function2.Name);
			Assert.AreEqual("test_function_1", function1_args.Name);
			Assert.AreEqual("test_function_2", function2_args.Name);
		}

		[TestMethod]
		public void Arguments()
		{

		}

		[TestMethod]
		public void ReturnValue()
		{
			var variables = new VariablesCache();
			var code = "def string test_function\n\treturn \"Hello world\"";
			var reader = LineReader.ParseLineWithChildren(code);

			var function = new Function(variables, reader);
			Assert.AreEqual("string", function.Type);
		}
	}
}
