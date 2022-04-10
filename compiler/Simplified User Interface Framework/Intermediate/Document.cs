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
		public readonly Element Body;
		public readonly Element RootElement = new Element();
		public readonly Dictionary<string, Macro> Macros = new Dictionary<string, Macro>();
		public readonly CodeBlock Script = new CodeBlock();


		public Document(DocumentReader reader)
		{
			try
			{
				// Start with macro and script parsing
				foreach (var section in reader.Sections)
				{
					if (section.Text.StartsWith("#"))
					{
						var macro = new Macro(section);
						Macros.Add(macro.Name, macro);
					}
					else if(section.First == "script")
					{
						foreach(var subSection in section.Children)
						{
							switch (subSection.First)
							{
								// Script function
								case Function.Declaration:
									{
										var words = new WordReader(subSection);
										var body = subSection.Children.Select(x => new WordReader(x)).ToArray();
										var function = new Function(words, body);
										if (Script.FunctionExists(function.Name))
											words.ThrowWordError(1, "Already defined", words.Length - 1);

										Script.Add(function);
									}
									break;

								case Variable.DynamicAccessType:
								case Variable.ReadOnlyAccessType:
									{
										var variable = new Variable(subSection.Text, subSection.LineNumber);
										if (Script.VariableExists(variable.Name))
											throw new Exception($"A variable named {variable.Name} already exists.");

										Script.Add(variable);
									}
									break;

								default:
									{
										var words = new WordReader(subSection);
										words.ThrowWordError(0, "Unknown keyword\nExpected a variable, function or data definition");
										break;
									}
							}
						}
					}
				}

				// Parse everything else
				foreach (var section in reader.Sections)
				{
					if (section.Text.StartsWith("#"))
						continue;

					switch (section.First)
					{
						case "script": continue;

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
							new WordReader(section).ThrowWordError(0, "Not implemented yet");
							break;

						case "body":
							Body = new Element(section);
							break;

						// Normal element parsing
						default:
							new WordReader(section).ThrowWordError(0, "Unknown keyword");
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
