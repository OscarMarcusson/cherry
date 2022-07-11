using System;
using System.Collections.Generic;
using System.Text;
using SimplifiedUserInterfaceFramework.Intermediate;

namespace SimplifiedUserInterfaceFramework
{
	class SharedCompilerInformation
	{
		readonly object ThreadLocker = new object();
		public Dictionary<string, Document> Documents = new Dictionary<string, Document>();


		public T GetValue<T>(Func<SharedCompilerInformation, T> selector)
		{
			lock (ThreadLocker)
				return selector(this);
		}

		public void DoThreadSafeWork(Action<SharedCompilerInformation> work)
		{
			lock (ThreadLocker)
				work(this);
		}
	}
}
