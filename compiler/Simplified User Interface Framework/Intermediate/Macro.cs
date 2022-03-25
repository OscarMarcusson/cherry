using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimplifiedUserInterfaceFramework.Intermediate;
using SimplifiedUserInterfaceFramework.Internal.Reader;

namespace SimplifiedUserInterfaceFramework.Internal.Intermediate
{
	public class Macro
	{
		public readonly string Name;
		public readonly Element[] Elements;


		public Macro(LineReader section)
		{
			Name = "#" + section.Text.Substring(1).Trim();
			Elements = section.Children.Select(x => new Element(x)).ToArray();
		}
	}
}
