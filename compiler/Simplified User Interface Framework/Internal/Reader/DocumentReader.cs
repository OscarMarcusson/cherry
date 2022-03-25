using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimplifiedUserInterfaceFramework.Internal.Reader
{
	public class DocumentReader
	{
		public readonly LineReader[] Sections;


		public DocumentReader(string path)
		{
			var file = File.ReadAllLines(path);
			var rootReaders = new List<LineReader>();

			LineReader previousReader = null;
			foreach (var line in file)
			{
				if (LineReader.TryReadLine(line, previousReader, out var reader))
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
