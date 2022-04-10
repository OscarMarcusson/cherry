using SimplifiedUserInterfaceFramework.Internal.Intermediate;
using SimplifiedUserInterfaceFramework.Internal.Reader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimplifiedUserInterfaceFramework.Intermediate
{
	public class Document
	{
		public readonly Style Style;
		public readonly Dictionary<string, Style> Styles = new Dictionary<string, Style>();
		public readonly Element Body;
		public readonly Element RootElement = new Element();
		public readonly Dictionary<string, Macro> Macros = new Dictionary<string, Macro>();
		public readonly string[] Includes;


		public Document(DocumentReader reader)
		{
			// Start with macro parsing
			foreach (var section in reader.Sections)
			{
				if (section.Text.StartsWith("#"))
				{
					var macro = new Macro(section);
					Macros.Add(macro.Name, macro);
				}
			}

			var includes = new List<string>();

			// Parse everything else
			foreach (var section in reader.Sections)
			{
				if (section.Text.StartsWith("#"))
					continue;

				switch (section.First)
				{
					case "include":
						{
							var space = section.Text.IndexOf(' ');
							if (space < 0)
								throw new Exception("No include value set");

							var include = section.Text.Substring(space).Trim();
							includes.Add(include);
						}
						break;

					// Script function
					case "def":
						{
							var functionName = section.Text.Substring(4).Trim();
							var index = functionName.IndexOfAny(new char[] { ' ', ':' });
							if (index > -1)
							{
								var functionArguments = functionName.Substring(index + 1).TrimStart();
								functionName = functionName.Substring(0, index);
							}
						}
						break;

					// Styles
					case "style":
						{
							var style = new Style(section);
							if (style.IsGlobal)
							{
								if (Style != null)
									throw new Exception("Already exists");

								Style = style;
							}
							else
							{
								if (Styles.TryGetValue(style.Name, out var existingStyle))
									throw new Exception("Already exists");

								Styles[style.Name] = style;
							}
						}
						break;

					case "head":
						throw new NotImplementedException("head not implemented");

					case "body":
						Body = new Element(section);
						break;

					// Normal element parsing
					default:
						throw new NotImplementedException("Unknown keyword: " +  section.First);
				}
			}

			// Make sure that the global style is initialized
			Style = Style ?? new Style();
			Includes = includes.ToArray();
		}
	}
}
