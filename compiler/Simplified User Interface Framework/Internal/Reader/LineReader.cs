using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimplifiedUserInterfaceFramework.Internal.Reader
{
	public class LineReader
	{
		public LineReader Parent { get; private set; }
		public readonly List<LineReader> Children = new List<LineReader>();

		public readonly int Indentation;
		public readonly string Text;
		public readonly string First;

		public LineReader(string line, LineReader parent = null)
		{
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

			var space = Text.IndexOf(' ');
			First = space > -1
					? Text.Substring(0, space)
					: Text;
		}



		public static bool TryReadLine(string line, LineReader previousLine, out LineReader reader)
		{
			if(string.IsNullOrWhiteSpace(line))
			{
				reader = null;
				return false;
			}

			reader = new LineReader(line);
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
	}
}
