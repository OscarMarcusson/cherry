using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cherry.Intermediate
{
	public class VariablesCache
	{
		public readonly VariablesCache Parent;
		readonly object Locker = new object();
		readonly Dictionary<string, Variable> Variables = new Dictionary<string, Variable>();

		const string NullGetWarning = "Can't get a variable using null as a key";
		const string NullSetWarning = "Can't set a variable using null as a key";

		// Helper getters
		public int Count => GetThreadSafeData(() => Variables.Count);
		public int RecursiveCount => Count + (Parent?.Count ?? 0);

		public string[] Names => GetThreadSafeData(() => Variables.Select(x => x.Key).ToArray());
		public Variable[] ToArray() => GetThreadSafeData(() => Variables.Select(x => x.Value).ToArray());


		T GetThreadSafeData<T>(Func<T> loader)
		{
			lock (Locker)
			{
				return loader();
			}
		}




		public VariablesCache(VariablesCache parent = null) => Parent = parent;


		public Variable this[string key]
		{
			get
			{
				if (key == null)
					throw new ArgumentNullException(NullGetWarning);

				lock (Locker)
				{
					if (Variables.TryGetValue(key, out var value))
						return value;

					return null;
				}
			}
			set
			{
				if (key == null)
					throw new ArgumentNullException(NullSetWarning);

				lock (Locker)
				{
					if(value == null)
						Variables.Remove(key);
				
					else
						Variables[key] = value;
				}
			}
		}


		public bool TryGetVariable(string key, out Variable variable)
		{
			if (key == null)
				throw new ArgumentNullException(NullGetWarning);

			lock (Locker)
				return Variables.TryGetValue(key, out variable);
		}


		public bool Exists(string key)
		{
			if (key == null)
				throw new ArgumentNullException(NullGetWarning);

			lock (Locker)
			{
				return Variables.ContainsKey(key);
			}
		}





		public bool TryGetVariableRecursive(string key, out Variable variable)
		{
			if (TryGetVariable(key, out variable))
				return true;

			if (Parent != null)
				return Parent.TryGetVariableRecursive(key, out variable);

			variable = null;
			return false;
		}

		public bool ExistsRecursive(string key) => Exists(key) || (Parent?.Exists(key) ?? false);




		// Helper variable creation methods
		public Variable Create(string raw, int lineNumber = -1) => new Variable(this, null, raw, lineNumber);
		public Variable Create(CodeLine parent, VariableType type, string name, object value) => new Variable(this, parent, type, name, value?.ToString() ?? "null");
		public void Remove(string variableName) => this[variableName] = null;
	}
}
