using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cherry.Intermediate;
using Cherry.Internal.Reader;

namespace Cherry.Internal.Intermediate
{
	// Disabling for now
	/*
	public class Macro
	{
		public readonly string Name;
		public readonly Element[] Elements;


		public Macro(LineReader section, CompilerArguments compilerArguments)
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
				var root = valueReader.ToElement();
				Elements = new[] { root };
			}
			// Multiple lines, like regular elements
			else
			{
				Elements = section.Children.Select(x => x.ToElement()).ToArray();
			}
		}
	}
	*/
}
