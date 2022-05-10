using System;
using System.Collections.Generic;
using System.Text;

namespace SimplifiedUserInterfaceFramework.Intermediate
{
	public enum DisplayLimit
	{
		Ignored = 0,
		Screen = 1 << 1,
		Print = 1 << 2,
		Voice = 1 << 3,
	}
}
