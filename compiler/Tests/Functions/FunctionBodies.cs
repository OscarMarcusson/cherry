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
	public class FunctionBodies
	{
		[TestMethod]
		public void CorrectNumberAndTypeOfChildren()
		{
			var variables = new VariablesCache();
			var code =	@"
						def test
							var a = 5
							a += 5
							print(a)
							return 5
						";
			var reader = LineReader.ParseLineWithChildren(code);
			var function = new Function(variables, reader);
			function.GenerateBody();
			Assert.IsNotNull(function.Body);
			Assert.AreEqual(4, function.Body.Length);
			Assert.IsInstanceOfType(function.Body[0], typeof(Variable));
			Assert.IsInstanceOfType(function.Body[1], typeof(VariableAssignment));
			Assert.IsInstanceOfType(function.Body[2], typeof(FunctionCall));
			Assert.IsInstanceOfType(function.Body[3], typeof(Return));
		}

		[TestMethod]
		public void LocalVariables()
		{
			var variables = new VariablesCache();
			var code = @"
						def test
							let a = 5
							return a
						";
			var reader = LineReader.ParseLineWithChildren(code);
			var function = new Function(variables, reader);
			function.GenerateBody();
			Assert.IsNotNull(function.Body);
			Assert.AreEqual(2, function.Body.Length);

			code = @"
						def test
							return this_var_does_not_exist
						";
			reader = LineReader.ParseLineWithChildren(code);
			Assert.ThrowsException<SectionException>(() => new Function(variables, reader).GenerateBody());
		}

		[TestMethod]
		public void Recursive()
		{
			throw new NotImplementedException();
		}

		[TestMethod]
		public void ConstantReturnValue()
		{
			var variables = new VariablesCache();
			var code = @"
						def i64 five
							return 5
						";
			var reader = LineReader.ParseLineWithChildren(code);
			var function = new Function(variables, reader);
			function.GenerateBody();
			Assert.IsNotNull(function.Body);
			Assert.AreEqual(1, function.Body.Length);
			Assert.AreEqual(typeof(Return), function.Body[0].GetType());
			Assert.AreEqual("5", ((Return)function.Body[0]).Value.Value);
		}

		[TestMethod]
		public void VariableReturnValue()
		{
			throw new NotImplementedException();
		}

		[TestMethod]
		public void FormulaReturnValue()
		{
			var variables = new VariablesCache();
			var code = @"
						def i64 test
							return 5 + (3 * 10)
						";
			var reader = LineReader.ParseLineWithChildren(code);
			var function = new Function(variables, reader);
			function.GenerateBody();
			Assert.IsNotNull(function.Body);
			Assert.AreEqual(1, function.Body.Length);
			Assert.AreEqual(typeof(Return), function.Body[0].GetType());
			Assert.AreEqual("35", ((Return)function.Body[0]).Value.Value);
			Assert.AreEqual(VariableValueType.Integer, ((Return)function.Body[0]).Value.Type);
		}

		[TestMethod]
		public void IncorrectReturnTypeThrowsError()
		{
			throw new NotImplementedException();
		}
	}
}
