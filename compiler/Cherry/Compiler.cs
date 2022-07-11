using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SimplifiedUserInterfaceFramework.Intermediate;
using SimplifiedUserInterfaceFramework.Internal;
using SimplifiedUserInterfaceFramework.Internal.Reader;

namespace SimplifiedUserInterfaceFramework
{
	public sealed class Compiler
	{
		readonly CompilerArguments Arguments;
		readonly SharedCompilerInformation SharedCompilerInformation;
		readonly bool IsRootDocument;
		readonly Log Log;
		readonly bool ThrowOnFail;

		public readonly string Input;
		public readonly string InputFileName;
		public readonly string InputDirectory;

		public readonly string Output;
		public readonly string OutputFileName;
		public readonly string OutputDirectory;



		public Compiler(CompilerArguments arguments, bool throwExceptionOnFail = false) : this(arguments, throwExceptionOnFail, new SharedCompilerInformation()) 
		{
			IsRootDocument = true;
		}

		private Compiler(CompilerArguments arguments, bool throwExceptionOnFail, SharedCompilerInformation sharedCompilerInformation)
		{
			IsRootDocument = false;
			Arguments = arguments.CreateCopy();
			SharedCompilerInformation = sharedCompilerInformation;
			Log = new Log { LogLevel = Arguments.LogLevel };
			ThrowOnFail = throwExceptionOnFail;

			Input  = Arguments.Input;
			Output = Arguments.Output;

			InputFileName  = Path.GetFileName(Input);
			InputDirectory  = Path.GetDirectoryName(Input);


			if (string.IsNullOrWhiteSpace(Arguments.Output))
			{
				Output = Path.ChangeExtension(Input, ".html");
				Log.Trace($"No output found, using: {Output}");
			}

			else if (Output.StartsWith("./"))
			{
				Output = Path.Combine(Path.GetDirectoryName(Input), Output.Substring(2));
				Log.Trace($"Relative output discovered, re-mapped to: {Output}");
			}
			
			OutputFileName = Path.GetFileName(Output);
			OutputDirectory = Path.GetDirectoryName(Output);
		}



		void Error(string message)
		{
			if (ThrowOnFail)
				throw new Exception(message);

			Log.Error(message);
		}



		public void Compile()
		{
			if (IsRootDocument)
				SharedCompilerInformation.Documents.Clear();

			// Validate
			if (!File.Exists(Input))
			{
				Error($"Input file not found: {Input}");
				return;
			}

			if(!Directory.Exists(OutputDirectory))
			{
				Error($"Output directory not found: {OutputDirectory}");
				return;
			}

			if (SharedCompilerInformation.GetValue(x => x.Documents.ContainsKey(Input)))
				return;

			try
			{
				DocumentReader reader = null;
				Document document = null;

				// We have to duplicate check again due to the time difference. If this document is not already compiled we add it while locked
				var alreadyExists = false;
				SharedCompilerInformation.DoThreadSafeWork(x =>
				{
					alreadyExists = x.Documents.ContainsKey(Input);
					if (!alreadyExists)
					{
						Log.Trace("Reading document...");
						reader = new DocumentReader(Input);
						document = new Document(reader, Arguments);
						x.Documents[Input] = document;
					}
				});

				if (alreadyExists)
					return;


				// Compile children
				Parallel.ForEach(document.ReferencedPages, new ParallelOptions { }, pageName =>
				{
					var targetExtension = Path.GetExtension(Input);
					var targetFile = Path.ChangeExtension(pageName, targetExtension);
					targetFile = Path.Combine(InputDirectory, targetFile);

					if (File.Exists(targetFile))
					{
						var argumentCopy = Arguments.CreateCopy();
						argumentCopy.Input = targetFile;
						argumentCopy.Output = Path.Combine(OutputDirectory, pageName);
						var compiler = new Compiler(argumentCopy, ThrowOnFail, SharedCompilerInformation);
						compiler.Compile();
					}
				});


				Log.Trace("Creating output file...");
				using (var file = File.Create(Output))
				using (var writer = new StreamWriter(file))
				{
					Log.Trace("Writing header...");
					writer.WriteLine("<!DOCTYPE html>");

					writer.WriteLine();
					writer.WriteLine("<head>");
					document.Meta.ToHtmlString(writer, 1);

					var fonts = new List<Include>();
					foreach(var link in document.Links)
					{
						switch (link.Type)
						{
							case IncludeType.CSS:        writer.WriteLine($"\t<link rel=\"stylesheet\" href=\"{link}\">"); break;
							case IncludeType.Javascript: writer.WriteLine($"\t<script src=\"{link}\"></script>");          break;

							case IncludeType.File: throw new NotImplementedException("No compiler implementation for " + Path.GetExtension(link.Value));

							case IncludeType.Directory: throw new NotImplementedException("No compiler implementation for directoreis");
							
							// Since fonts are added in the CSS document we have to skip them here, but add them to the list to make it easier to check if a style should be written
							case IncludeType.Font: 
								fonts.Add(link); 
								break;

							default: throw new NotImplementedException("No compiler implementation for including " + link.Type);
						}
					}

					foreach (var style in document.Styles)
					{
						Log.Trace($"Adding {style.Key} style...");
						writer.WriteLine();
						writer.WriteLine($"\t<!-- {style.Key} style  -->");
						writer.WriteLine($"\t<style type=\"text/css\" rel=\"{style.Key.ToLower()}\" title=\"{style.Key}\">");
						style.Value.ToCssStream(writer, 2);
						writer.WriteLine("\t</style>");
					}

					if (document.Style.Elements.Count() > 0 || document.Style.MediaQueries.Count > 0 || document.Styles.Count > 0 || document.IncludeStyles.Length > 0 || fonts.Count > 0)
					{
						Log.Trace($"Adding global style...");
						writer.WriteLine();
						writer.WriteLine("\t<!-- Global style  -->");
						writer.WriteLine("\t<style>");

						if(fonts.Count > 0)
						{
							foreach(var font in fonts)
							{
								writer.WriteLine("\t\t@font-face {");
								writer.WriteLine($"\t\t\tfont-family: {Path.GetFileNameWithoutExtension(font.Value)};");
								writer.WriteLine($"\t\t\tsrc: url({font.Value});");
								writer.WriteLine("\t\t}");
							}
						}

						foreach (var include in document.IncludeStyles)
							include.ToStream(writer, 2, InputDirectory);

						document.Style.ToCssStream(writer, 2);
						writer.WriteLine("\t</style>");
					}
					writer.WriteLine("</head>");


					Log.Trace("Writing body...");
					writer.WriteLine();
					document.Body.ToStartHtmlStream(writer, document, 0);
					document.Body.WriteContentToHtml(writer, document, 1);

					if (document.Script.HasContent || document.ContainsFrameworkCode || document.Bindings.Count > 0 || document.IncludesScripts.Length > 0 || document.CustomElements.Count > 0)
					{
						writer.WriteLine("\t<script>");

						// Bindings
						document.BindingsToJavascriptStream(writer, 2);

						// Variables
						var variables = document.Script.GetVariables();
						if (variables.Length > 0)
						{
							foreach (var variable in variables)
								variable.ToJavascriptStream(writer, 2);

							writer.WriteLine();
						}

						// Custom elements
						if (document.CustomElements.Count > 0)
						{
							writer.WriteLine("\t\t// Custom elements");
							foreach (var element in document.CustomElements)
							{
								element.Value.ToJavascriptClass(writer, 2);
							}
						}

						// Framework code
						if (document.ContainsFrameworkCode)
						{
							writer.WriteLine("\t\t// Framework code");
							// Set defauls on load
							// TODO:: Loop through each tab selector group and click the default (or first) button

							// Select tab function
							var selectTab = CompilerResources.GetJavascript("Tabs");
							writer.WriteLine("\t\t" + selectTab.Replace("\n", "\n\t\t"));
						}

						// Functions
						var functions = document.Script.GetFunctions();
						if (functions.Length > 0)
						{
							foreach (var function in functions)
								function.ToJavascriptStream(writer, 2);
						}

						// Embedded code from other files
						if (document.IncludesScripts.Length > 0)
						{
							writer.WriteLine("\t\t// Include scripts");
							foreach (var include in document.IncludesScripts)
								include.ToStream(writer, 2, InputDirectory);
						}

						writer.WriteLine("\t</script>");
					}
					document.Body.ToEndHtmlStream(writer, 0);
				}

				Log.Trace("Done");
			}
			catch(SectionException e)
			{
				Log.SectionError(e);
			}
		}

		public Document[] GetDocuments() => SharedCompilerInformation.GetValue(x => x.Documents.Select(d => d.Value).ToArray());
	}
}
