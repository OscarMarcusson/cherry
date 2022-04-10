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
			Name = "#" + section.Text.Substring(1).TrimStart();

			var index = Name.IndexOf('=');
			
			// Single line? Like #hw = div > p = Hello World!
			if(index > 0)
			{
				if (section.Children.Count > 0)
					throw new Exception($"The single line macro \"{section}\" has child content.");

				var value = Name.Substring(index+1).Trim();
				Name = Name.Substring(0, index).TrimEnd();

				var valueReader = new LineReader(value, lineNumber:section.LineNumber);
				var root = new Element(valueReader);
				Elements = new[] { root };
			}
			// Multiple lines, like regular elements
			else
			{
				Elements = section.Children.Select(x => new Element(x)).ToArray();
			}
		}
	}
}
