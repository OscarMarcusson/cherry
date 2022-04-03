﻿using System;
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
				var test = new Variable("plap plop plep");
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

					// writer.WriteLine("\t<script>");
					// writer.WriteLine("\t</script>");
					writer.WriteLine("</head>");


					Log.Trace("Writing body...");
					writer.WriteLine();
					writer.WriteLine("<body>");
					foreach (var element in document.RootElement.Children)
						element.ToRecursiveHtmlStream(writer, document, Log);
					writer.WriteLine("</body>");
				}

				Log.Trace("Done");
			}
			catch(SectionException sectionException)
			{
				Log.SectionError(sectionException.Left, sectionException.Center, sectionException.Right, sectionException.Message);
			}
		}
	}
}
