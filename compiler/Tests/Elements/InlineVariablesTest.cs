using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimplifiedUserInterfaceFramework;
using SimplifiedUserInterfaceFramework.Intermediate;
using SimplifiedUserInterfaceFramework.Internal;
using SimplifiedUserInterfaceFramework.Internal.Reader;

namespace Parser
{
	[TestClass]
	public class InlineVariablesTest
	{
		[TestMethod]
		public void CanInsertConstants()
		{
			var variables = new VariablesCache();
			var variable = variables.Create("let test = \"some-value\"");

			var root = new LineReader("div").ToElement(variables);
			var child = root.AddChild("p = " + variable.Name);

			Assert.AreEqual(variable.Value.Value.ToString(), child.Value);
		}
	}
}
