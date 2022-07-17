using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Cherry.Intermediate
{
	public enum IncludeType
	{
		Javascript,
		CSS,
		File,
		Directory,
		Font,
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
					case ".ttf": Type = IncludeType.Font;       break;
					default:     Type = IncludeType.File;       break;
				}
			}
			else
			{
				Type = IncludeType.Directory;
			}
		}

		public override string ToString() => Value;



		public void ToStream(StreamWriter writer, int indentation, string root)
		{
			if (Type == IncludeType.Directory)
				throw new NotSupportedException("Can't embedd a directory");

			if(Type == IncludeType.Font)
				throw new NotSupportedException("Can't embedd a font (yet)");

			var path = Path.Combine(root, Value);
			if (!File.Exists(path))
				throw new FileNotFoundException("Can't embedd missing file: " + Value);

			var raw = File.ReadAllText(path);
			if (indentation > 0)
			{
				var indentationString = indentation > 0 ? new string('\t', indentation) : "";
				raw = indentationString + raw.Replace("\r", "").Replace("\n\n", "\n").Replace("\n\n", "\n").Replace("\n", $"{Environment.NewLine}{indentationString}");
			}

			writer.WriteLine(raw);
		}
	}
}
