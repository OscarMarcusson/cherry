using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cherry.Internal.Reader
{
	public class DocumentReader
	{
		public readonly LineReader[] Sections;
		public readonly string File;


		public DocumentReader(string path)
		{
			File = path;
			var file = System.IO.File.ReadAllLines(path);
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
