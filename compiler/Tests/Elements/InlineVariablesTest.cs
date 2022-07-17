using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cherry;
using Cherry.Intermediate;
using Cherry.Internal;
using Cherry.Internal.Reader;

namespace Parser
{
	[TestClass]
	public class InlineVariablesTest
	{
		[TestMethod]
		public void ConstantLiterals()
		{
			var variables = new VariablesCache();
			var variable1 = variables.Create("let test1 = \"some-value\"");
			var variable2 = variables.Create("let test2 = 12345");

			var root = new LineReader("div").ToElement(variables);
			var child1 = root.AddChild("p = " + variable1.Name);
			var child2 = root.AddChild("p = " + variable2.Name);

			Assert.AreEqual(variable1.Value.Value.ToString(), child1.Value);
			Assert.AreEqual(variable2.Value.Value.ToString(), child2.Value);
		}


		[TestMethod]
		public void StringInterpolation()
		{
			var variables = new VariablesCache();
			var variable1 = variables.Create("let test1 = \"some-value\"");
			var variable2 = variables.Create("let test2 = 12345");
			var interpolationString = "Test1: {" + variable1.Name + "}\nTest2: {" + variable2.Name + "}";
			var interpolationResult = $"Test1: {variable1.Value.Value}\nTest2: {variable2.Value.Value}";

			var root = new LineReader("div").ToElement(variables);
			var child = root.AddChild($"p = \"{interpolationString}\"");

			Assert.AreEqual(interpolationResult, child.Value);
		}
	}
}
