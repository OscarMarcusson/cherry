using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cherry.Internal.Reader
{
	public class LineReader
	{
		public LineReader Parent { get; private set; }
		public readonly List<LineReader> Children = new List<LineReader>();

		public readonly int LineNumber;
		public readonly int Indentation;
		public readonly string Text;
		public readonly string First;

		public LineReader(string line, LineReader parent = null, int lineNumber = -1)
		{
			LineNumber = lineNumber;

			if(parent != null)
			{
				Parent = parent;
				Parent.Children.Add(this);
			}

			for(int i = 0; i < line.Length; i++)
			{
				if(line[i] != '\t')
				{
					Indentation = i;
					Text = line.Substring(i);
					break;
				}
			}
			if (Text == null)
				Text = "";

			var space = Text.IndexOf(' ');
			First = space > -1
					? Text.Substring(0, space)
					: Text;
		}





		public static bool TryReadLine(string line, LineReader previousLine, int lineNumber, out LineReader reader)
		{
			if(string.IsNullOrWhiteSpace(line))
			{
				reader = null;
				return false;
			}

			reader = new LineReader(line, lineNumber:lineNumber);
			if (reader.First.StartsWith("//"))
			{
				reader = null;
				return false;
			}

			if (previousLine != null && reader.Indentation > 0)
			{
				if(reader.Indentation > previousLine.Indentation)
				{
					reader.Parent = previousLine;
					reader.Parent.Children.Add(reader);
				}
				else if(reader.Indentation == previousLine.Indentation)
				{
					if(previousLine.Parent != null)
					{
						reader.Parent = previousLine.Parent;
						reader.Parent.Children.Add(reader);
					}
				}
				else
				{
					while (previousLine != null && previousLine.Indentation >= reader.Indentation)
						previousLine = previousLine.Parent;

					if(previousLine != null)
					{
						reader.Parent = previousLine;
						reader.Parent.Children.Add(reader);
					}
				}
			}

			return true;
		}


		public override string ToString() => Text;



		public static LineReader ParseLineWithChildren(string raw)
		{
			var lines = ParseLines(raw);
			if (lines.Length == 1)
				return lines[0];

			throw new ArgumentException("Could not parse as single linereader, too many root lines. Expected the first line to have no indentation and the rest to have at least 1 indentation:\n" + raw);
		}

		public static LineReader[] ParseLines(string raw)
		{
			if (string.IsNullOrWhiteSpace(raw))
				return new[] { new LineReader("") };

			var split = raw.Replace("\r", "").Split('\n');
			var output = new List<LineReader>();
			LineReader parentReader = null;
			foreach (var line in split)
			{
				if (string.IsNullOrWhiteSpace(line))
					continue;

				if(parentReader == null)
				{
					parentReader = new LineReader(line);
					output.Add(parentReader);
				}
				else
				{
					var newReader = new LineReader(line);
					if(newReader.Indentation == 0)
					{
						output.Add(newReader);
						parentReader = newReader;
					}
					else
					{
						while (parentReader != null && parentReader.Indentation >= newReader.Indentation)
							parentReader = parentReader.Parent;

						if (parentReader != null)
						{
							parentReader.Children.Add(newReader);
							newReader.Parent = parentReader;
							parentReader = newReader;
						}
						else
						{
							output.Add(newReader);
							parentReader = newReader;
						}
					}
				}
			}
			return output.ToArray();
		}
	}
}
