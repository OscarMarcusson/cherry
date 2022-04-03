using SimplifiedUserInterfaceFramework.Internal.Intermediate;
using SimplifiedUserInterfaceFramework.Internal.Reader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimplifiedUserInterfaceFramework.Intermediate
{
	public class Document
	{
		public readonly Style Style;
		public readonly Dictionary<string, Style> Styles = new Dictionary<string, Style>();
		public readonly Element RootElement = new Element();
		public readonly Dictionary<string, Macro> Macros = new Dictionary<string, Macro>();
		public readonly CodeBlock Script = new CodeBlock();


		public Document(DocumentReader reader)
		{
			try
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

				// Parse everything else
				foreach (var section in reader.Sections)
				{
					if (section.Text.StartsWith("#"))
						continue;

					switch (section.First)
					{
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

						case Variable.DynamicAccessType:
						case Variable.ReadOnlyAccessType:
							{
								var variable = new Variable(section.Text, section.LineNumber);
								if(Script.VariableExists(variable.Name))
									throw new Exception($"A variable named {variable.Name} already exists.");

								Script.Add(variable);
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

						// Normal element parsing
						default:
							RootElement.AddChild(section);
							break;
					}
				}

				// Make sure that the global style is initialized
				Style = Style ?? new Style();
			}
			catch(SectionException e)
			{
				// Re-throw exception with more information added
				throw new SectionException(e.Left, e.Center, e.Right, e.Message, e.LineNumber, Path.GetFileName(reader.File));
			}
		}
	}
}
