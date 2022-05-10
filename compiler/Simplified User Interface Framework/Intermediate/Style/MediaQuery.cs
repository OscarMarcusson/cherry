using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SimplifiedUserInterfaceFramework.Internal.Reader;

namespace SimplifiedUserInterfaceFramework.Intermediate
{
	public struct MediaQuery
	{
		public readonly DisplayLimit DisplayLimit;
		public readonly int MinWidth;
		public readonly int MaxWidth;
		public readonly int MinHeight;
		public readonly int MaxHeight;



		public MediaQuery(string source) : this(new LineReader(source)) { }

		public MediaQuery(LineReader valueReader)
		{
			DisplayLimit = DisplayLimit.Ignored;
			MinWidth = 0;
			MaxWidth = 0;
			MinHeight = 0;
			MaxHeight = 0;

			var reader = new WordReader(valueReader);

			if (reader.First == "else")
				reader.ThrowWordError(0, "Not implemented yet");

			// TODO:: Implement proper reading with (sections & (stuff))
			for (int i = 1; i < reader.Length; i++)
			{
				switch (reader[i])
				{
					case "screen": DisplayLimit |= DisplayLimit.Screen; break;
					case "print": DisplayLimit |= DisplayLimit.Print; break;
					case "voice": DisplayLimit |= DisplayLimit.Voice; break;

					// Ignore for now
					case "and":
					case "&&":
					case "not":
					case "!":
						break;

					case "width":
					case "height":
						var isWidth = reader[i++] == "width";
						var comparitor = reader[i++];
						var type = reader[i].EndsWith("px")
										? "px"
										: reader[i].EndsWith("em")
											? "em"
											: "px"
											;
						var value = int.Parse(new string(reader[i].Where(x => char.IsDigit(x)).ToArray()));

						switch (comparitor)
						{
							case ">=": if (isWidth) MinWidth = value;     else MinHeight = value;     break;
							case ">":  if (isWidth) MinWidth = value + 1; else MinHeight = value + 1; break;
							case "<=": if (isWidth) MaxWidth = value;     else MaxHeight = value;     break;
							case "<":  if (isWidth) MaxWidth = value - 1; else MaxHeight = value - 1; break;
							case "=":  if (isWidth) MaxWidth = MinWidth = value; else MinHeight = MinHeight = value; break;
								// TODO:: Implement later, needs an OR between them
								// case "!=": mediaElement.MaxWidth = mediaElement.MinWidth = value; break;
						}

						break;

					default:
						reader.ThrowWordError(i, "Unknown query option");
						break;
				}
			}
		}



		public static MediaQuery Empty = new MediaQuery();

		internal void ToStartCssStream(StreamWriter writer, string indent)
		{
			writer.Write(indent);
			writer.Write("@media ");
			if (DisplayLimit != DisplayLimit.Ignored)
			{
				var limits = new[]
					{
								DisplayLimit.HasFlag(DisplayLimit.Screen) ? "screen" : null,
								DisplayLimit.HasFlag(DisplayLimit.Print) ? "print" : null,
								DisplayLimit.HasFlag(DisplayLimit.Voice) ? "voice" : null,
							}
					.Where(x => x != null)
					;

				writer.Write(string.Join(" and ", limits));
			}

			var hasAnySize = MinWidth > 0 || MaxWidth > 0 || MinHeight > 0 || MaxHeight > 0;
			if (hasAnySize)
			{
				if (DisplayLimit != DisplayLimit.Ignored)
					writer.Write(" and");

				var sizes = new[]
					{
								MinWidth > 0 ? $"(min-width:{MinWidth}px)" : null,
								MaxWidth > 0 ? $"(max-width:{MaxWidth}px)" : null,
								MinHeight > 0 ? $"(min-height:{MinHeight}px)" : null,
								MaxHeight > 0 ? $"(max-height:{MaxHeight}px)" : null,
							}
					.Where(x => x != null)
					;
				writer.Write(string.Join(" and ", sizes));
			}
			writer.WriteLine(" {");
		}




		internal void ToEndCssStream(StreamWriter writer, string indent)
		{
			writer.Write(indent);
			writer.WriteLine('}');
		}
	}
}
