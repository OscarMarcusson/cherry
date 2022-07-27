using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cherry;
using Cherry.Intermediate;

namespace Functions
{
	[TestClass]
	public class VariableAssignments
	{
		[TestMethod]
		public void VariableNotFound()
		{
			var variables = new VariablesCache();
			var variable = new Variable(variables, null, "var a = 5");
			Assert.ThrowsException<SectionException>(() => new VariableAssignment(variables, null, "doesnt_exist += 5"));
		}

		[TestMethod]
		public void IncorrectTypesThrows()
		{
			var variables = new VariablesCache();
			var variable = new Variable(variables, null, "var a = 5");
			Assert.ThrowsException<SectionException>(() => new VariableAssignment(variables, null, "a += \"error\""));
		}

		[TestMethod]
		public void CompileTimeLiteralValueEvaluation()
		{
			var variables = new VariablesCache();
			var variable = new Variable(variables, null, "var a = 5");
			var assignment = new VariableAssignment(variables, null, "a += 5");

			Assert.AreEqual("10", variable.Value.Value);
		}

		[TestMethod]
		public void VariableValue()
		{

		}

		[TestMethod]
		public void NoSpaceParsing()
		{
			var variables = new VariablesCache();
			var variable = new Variable(variables, null, "var a = 5");
			var assignment = new VariableAssignment(variables, null, "a+=5 * 10");

			Assert.AreEqual("a", assignment.Name);
			Assert.AreEqual("55", variable.Value.Value);
		}
	}
}
