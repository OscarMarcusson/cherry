using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SimplifiedUserInterfaceFramework.Intermediate
{
	public enum IncludeType
	{
		Javascript,
		CSS,
		File,
		Directory
	}


	public class Include
	{
		public readonly string Value;
		public readonly IncludeType Type;

		public Include(string path)
		{
			path = path.Trim(' ', '\t', '"', '\'');
			Value = path;

			if (Path.HasExtension(path))
			{
				switch (Path.GetExtension(path))
				{
					case ".css": Type = IncludeType.CSS;        break;
					case ".js":  Type = IncludeType.Javascript; break;
					default:     Type = IncludeType.File;       break;
				}
			}
			else
			{
				Type = IncludeType.Directory;
			}
		}

		public override string ToString() => Value;
	}
}
