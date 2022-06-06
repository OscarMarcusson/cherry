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
			var root = new LineReader("div").ToElement();
			var variable = new Variable("let test = some-value");
			root.AddVariable(variable);

			var child = root.AddChild("p = " + variable.Name);
			Assert.AreEqual(variable.Value.ToString(), child.Value);
		}
	}
}
