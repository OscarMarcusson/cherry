using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cherry.Intermediate;

namespace Variables
{
	[TestClass]
	public class VariablesCacheTests
	{
		[TestMethod]
		public void CanUseIndex()
		{
			var cache = new VariablesCache();
			
			var a = new Variable(cache, VariableType.ReadOnly, "a", "5");
			Assert.AreEqual(a, cache["a"]);

			// And to ensure that we can also set it, even though it's done automatically internally
			cache["a"] = a;
			Assert.AreEqual(a, cache["a"]);
		}


		[TestMethod]
		public void CanUseTryGet()
		{
			var cache = new VariablesCache();
			var a = new Variable(cache, VariableType.ReadOnly, "a", "5");

			Assert.AreEqual(true, cache.TryGetVariable("a", out var foundA));
			Assert.AreEqual(a, foundA);

			Assert.AreEqual(false, cache.TryGetVariable("b", out var foundB));
			Assert.AreEqual(null, foundB);
		}


		[TestMethod]
		public void CanUseExists()
		{
			var cache = new VariablesCache();
			new Variable(cache, VariableType.ReadOnly, "a", "5");

			Assert.AreEqual(true, cache.Exists("a"));
			Assert.AreEqual(false, cache.Exists("b"));
		}


		[TestMethod]
		public void CanRemove()
		{
			var cache = new VariablesCache();
			new Variable(cache, VariableType.ReadOnly, "a", "5");

			cache["a"] = null;

			Assert.AreEqual(null, cache["a"]);
			Assert.AreEqual(false, cache.Exists("a"));
		}


		[TestMethod]
		public void GetRecursively()
		{
			var rootCache = new VariablesCache();
			var a = new Variable(rootCache, VariableType.ReadOnly, "a", "5");

			var cache = new VariablesCache(rootCache);

			Assert.AreEqual(null, cache["a"]);

			Assert.AreEqual(true, cache.TryGetVariableRecursive("a", out var foundA));
			Assert.AreEqual(a, foundA);
		}


		[TestMethod]
		public void ExistsCheckRecursively()
		{
			var rootCache = new VariablesCache();
			var a = new Variable(rootCache, VariableType.ReadOnly, "a", "5");

			var cache = new VariablesCache(rootCache);

			Assert.AreEqual(false, cache.Exists("a"));
			Assert.AreEqual(true, cache.ExistsRecursive("a"));
		}
	}
}
