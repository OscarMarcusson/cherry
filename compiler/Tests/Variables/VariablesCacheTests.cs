using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimplifiedUserInterfaceFramework.Intermediate;

namespace Variables
{
	[TestClass]
	public class VariablesCacheTests
	{
		[TestMethod]
		public void CanUseIndex()
		{
			var cache = new VariablesCache();
			
			var a = new Variable(VariableType.ReadOnly, "a", "5");
			cache["a"] = a;
			Assert.AreEqual(a, cache["a"]);
		}


		[TestMethod]
		public void CanUseTryGet()
		{
			var cache = new VariablesCache();

			var a = new Variable(VariableType.ReadOnly, "a", "5");
			cache["a"] = a;

			Assert.AreEqual(true, cache.TryGetVariable("a", out var foundA));
			Assert.AreEqual(a, foundA);

			Assert.AreEqual(false, cache.TryGetVariable("b", out var foundB));
			Assert.AreEqual(null, foundB);
		}


		[TestMethod]
		public void CanUseExists()
		{
			var cache = new VariablesCache();

			cache["a"] = new Variable(VariableType.ReadOnly, "a", "5");

			Assert.AreEqual(true, cache.Exists("a"));
			Assert.AreEqual(false, cache.Exists("b"));
		}


		[TestMethod]
		public void CanRemove()
		{
			var cache = new VariablesCache();

			var a = new Variable(VariableType.ReadOnly, "a", "5");
			cache["a"] = a;
			cache["a"] = null;

			Assert.AreEqual(null, cache["a"]);
			Assert.AreEqual(false, cache.Exists("a"));
		}
	}
}
