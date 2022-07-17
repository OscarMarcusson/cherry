using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cherry.Internal
{
	internal static class ArgumentPrinter
	{
		const int KeyWidth = 30;
		const int DescriptionWidth = MaxWidth - KeyWidth;
		const int MaxWidth = 80;

		public const ConsoleColor TitleColor = ConsoleColor.White;

		public const ConsoleColor KeyColor = ConsoleColor.White;
		public const ConsoleColor SyntaxHelperColor = ConsoleColor.DarkGray;
		public const ConsoleColor DescriptionColor = ConsoleColor.Gray;




		public static void PrintArgumentUsage(string example)
		{
			Console.ForegroundColor = TitleColor;
			Console.Write("Usage: ");

			Console.ForegroundColor = DescriptionColor;
			Console.WriteLine(example);

			Console.ResetColor();
		}

		public static void PrintArgumentExample(string example)
		{
			Console.ForegroundColor = TitleColor;
			Console.Write("Example: ");

			Console.ForegroundColor = DescriptionColor;
			Console.WriteLine(example);

			Console.ResetColor();
		}


		public static void PrintArgumentDescription(string shortKey, string longKey, string value, string description)
		{
			Console.Write("    ");

			Console.ForegroundColor = KeyColor;
			if (shortKey != null && longKey != null)
			{
				Console.Write($"-{shortKey}");

				Console.ForegroundColor = SyntaxHelperColor;
				Console.Write(" | ");

				Console.ForegroundColor = KeyColor;
				Console.Write($"--{longKey}");
			}
			else if(shortKey != null)
			{
				Console.Write($"-{shortKey}");
			}
			else if(longKey != null)
			{
				Console.Write($"--{longKey}");
			}

			if(value != null)
			{
				Console.ForegroundColor = SyntaxHelperColor;
				Console.Write(" <");

				Console.ForegroundColor = KeyColor;
				Console.Write(value);

				Console.ForegroundColor = SyntaxHelperColor;
				Console.Write('>');
			}

			Console.ForegroundColor = DescriptionColor;
			PrintDescription(description);

			Console.ResetColor();
			Console.CursorLeft = 0;
		}






		static void PrintDescription(string description)
		{
			Console.CursorLeft = KeyWidth;
			if (description.Length > DescriptionWidth)
			{
				var lines = description.Split('\n');

				foreach(var line in lines)
				{
					if (line.Length > DescriptionWidth)
					{
						PrintSplitDescriptionLine(line);
					}
					else
					{
						Console.WriteLine(line);
						Console.CursorLeft = KeyWidth;
					}
				}
			}
			else
			{
				Console.WriteLine(description);
			}
		}

		private static void PrintSplitDescriptionLine(string line)
		{
			var index = 0;
			var previousIndex = 0;
			while (index > -1)
			{
				index = line.IndexOf(' ', previousIndex+1);
				if (index >= DescriptionWidth)
				{
					var sectionToPrint = line.Substring(0, previousIndex).TrimEnd();
					line = line.Substring(previousIndex).TrimStart();
					index = 0;

					Console.WriteLine(sectionToPrint);
					Console.CursorLeft = KeyWidth;
				}
				else if(index < 0)
				{
					if(line.Length >= DescriptionWidth)
					{
						Console.WriteLine(line.Substring(0, previousIndex));
						Console.CursorLeft = KeyWidth;
						Console.WriteLine(line.Substring(previousIndex + 1).TrimStart());
						Console.CursorLeft = KeyWidth;
					}
					else
					{
						Console.WriteLine(line);
						Console.CursorLeft = KeyWidth;
					}
				}

				previousIndex = index;
			}			
		}
	}
}
