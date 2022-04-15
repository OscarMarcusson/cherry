using System;
using System.IO;
using System.Linq;
using SimplifiedUserInterfaceFramework.Intermediate;
using SimplifiedUserInterfaceFramework.Internal.Reader;

namespace SimplifiedUserInterfaceFramework
{
	public sealed class Compiler
	{
		readonly CompilerArguments Arguments;
		readonly Log Log;
		readonly bool ThrowOnFail;

		public readonly string Input;
		public readonly string InputFileName;
		public readonly string InputDirectory;

		public readonly string Output;
		public readonly string OutputFileName;
		public readonly string OutputDirectory;



		public Compiler(CompilerArguments arguments, bool throwExceptionOnFail = false)
		{
			Arguments = arguments.CreateCopy();
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

			try
			{
				// Compile
				Log.Trace("Reading document...");
				var reader = new DocumentReader(Input);
				var document = new Document(reader);

				Log.Trace("Creating output file...");
				using (var file = File.Create(Output))
				using (var writer = new StreamWriter(file))
				{
					Log.Trace("Writing header...");
					writer.WriteLine("<!DOCTYPE html>");

					writer.WriteLine();
					writer.WriteLine("<head>");
					writer.WriteLine("\t<title>Hello World title</title>");

					foreach(var include in document.Includes)
					{
						switch (include.Type)
						{
							case IncludeType.CSS:        writer.WriteLine($"\t<link rel=\"stylesheet\" href=\"{include}\">"); break;
							case IncludeType.Javascript: writer.WriteLine($"\t<script src=\"{include}\"></script>");          break;

							case IncludeType.File: throw new NotImplementedException("No compiler implementation for " + Path.GetExtension(include.Value));

							case IncludeType.Directory: throw new NotImplementedException("No compiler implementation for directoreis");

							default: throw new NotImplementedException("No compiler implementation for including " + include.Type);
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

					if (document.Style.Elements.Count() > 0 || document.Styles.Count > 0)
					{
						Log.Trace($"Adding global style...");
						writer.WriteLine();
						writer.WriteLine("\t<!-- Global style  -->");
						writer.WriteLine("\t<style>");
						document.Style.ToCssStream(writer, 2);
						writer.WriteLine("\t</style>");
					}

					if (document.Script.HasContent || document.Bindings.Count > 0)
					{
						writer.WriteLine();
						writer.WriteLine("\t<script>");

						document.BindingsToJavascriptStream(writer, 2);

						var variables = document.Script.GetVariables();
						if(variables.Length > 0)
						{
							foreach (var variable in variables)
								variable.ToJavascriptStream(writer, 2);
							
							writer.WriteLine();
						}

						var functions = document.Script.GetFunctions();
						if(functions.Length > 0)
						{
							foreach(var function in functions)
								function.ToJavascriptStream(writer, 2);
						}

						writer.WriteLine("\t</script>");
					}

					writer.WriteLine("</head>");


					Log.Trace("Writing body...");
					writer.WriteLine();

					Log.Trace("Writing body...");
					writer.WriteLine();
					document.Body.ToRecursiveHtmlStream(writer, document, Log);
				}

				Log.Trace("Done");
			}
			catch(SectionException e)
			{
				Log.SectionError($"Failed to parse {e.FileName}{(e.LineNumber > -1 ? $"\nLine {e.LineNumber}" : "")}\n{e.Left}", e.Center, e.Right, e.Message);
			}
		}
	}
}
