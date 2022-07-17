using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cherry.Internal.Reader
{
	public enum SourceType
	{
		Path,
		Raw
	}

	public class DocumentReader
	{
		public readonly LineReader[] Sections;
		public readonly string File;


		public DocumentReader(string path) : this(path, SourceType.Path) { }

		public static DocumentReader Raw(params string[] raw) => new DocumentReader(string.Join("\n", raw), SourceType.Raw);

		DocumentReader(string value, SourceType sourceType)
		{
			string[] file = null;

			if(sourceType == SourceType.Path)
			{
				File = value;
				file = System.IO.File.ReadAllLines(value);
			}
			else
			{
				file = value.Replace("\r", "").Split("\n");
			}

			var rootReaders = new List<LineReader>();

			LineReader previousReader = null;
			for(int lineNumber = 0; lineNumber < file.Length; lineNumber++)
			{
				if (LineReader.TryReadLine(file[lineNumber], previousReader, lineNumber+1, out var reader))
				{
					previousReader = reader;

					if (reader.Indentation == 0)
						rootReaders.Add(reader);
				}
			}
			Sections = rootReaders.ToArray();
		}
	}
}
