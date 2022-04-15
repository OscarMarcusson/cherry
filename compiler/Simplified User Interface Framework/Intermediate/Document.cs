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
		public readonly DocumentReader Source;
		public readonly Style Style;
		public readonly Dictionary<string, Style> Styles = new Dictionary<string, Style>();
		public readonly Element Body;
		public readonly Dictionary<string, Macro> Macros = new Dictionary<string, Macro>();
		public readonly CodeBlock Script = new CodeBlock();
		public readonly Include[] Links;
		public readonly Include[] IncludeStyles;
		public readonly Include[] IncludesScripts;
		public readonly Dictionary<string, string> Bindings = new Dictionary<string, string>();


		public Document(DocumentReader reader)
		{
			Source = reader;
			var includes = new List<Include>();
			var links = new List<Include>();

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

						case "link":
							{
								var value = GetIncludeOrLinkValue(section);
								links.Add(new Include(value));
							}
							break;
						case "include":
							{
								var value = GetIncludeOrLinkValue(section);
								includes.Add(new Include(value));
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

			// Make sure that the global style is initialized
			Style = Style ?? new Style();
			IncludeStyles = includes.Where(x => x.Type == IncludeType.CSS).ToArray();
			IncludesScripts = includes.Where(x => x.Type == IncludeType.Javascript).ToArray();

			// TODO:: This is some temp stuff, should be removed later when everything is implemented properly
			var invalid = includes.Where(x => x.Type != IncludeType.CSS && x.Type != IncludeType.Javascript);
			if(invalid.Count() > 0)
				throw new NotImplementedException("Include not implemented for:\n * " + string.Join("\n * ", invalid.Select(x => x.Value)));
			
			Links = links.ToArray();

			// Post processing
			Body?.ResolveBindings(this);
		}


		string GetIncludeOrLinkValue(LineReader line)
		{
			var raw = line.Text.TrimEnd();
			var space = raw.IndexOf(' ');
			if (space < 0)
				new WordReader(line).ThrowWordError(1, "No value found");

			var value = raw.Substring(space).Trim();
			return value;
		}


		public void BindingsToJavascriptStream(StreamWriter writer, int indent = 0)
		{
			if (Bindings.Count > 0)
			{
				var indentString = indent > 0 ? new string('\t', indent) : "";

				// Declarations
				writer.WriteLine($"{indentString}// Element binding declarations");
				foreach (var binding in Bindings)
					writer.WriteLine($"{indentString}let {binding.Value};");

				// Bindings
				writer.WriteLine();
				writer.WriteLine($"{indentString}window.onload = OnLoadWindow;");
				writer.WriteLine($"{indentString}function OnLoadWindow() {{");
				writer.WriteLine($"{indentString}\t// Element bindings");
				foreach (var binding in Bindings)
					writer.WriteLine($"{indentString}\t{binding.Value} = document.getElementById('{binding.Key}');");
				writer.WriteLine($"{indentString}}}");
			}
		}
	}
}
