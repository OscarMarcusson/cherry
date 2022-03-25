using System;
using System.IO;
using System.Linq;
using SimplifiedUserInterfaceFramework.Internal;

namespace SimplifiedUserInterfaceFramework
{
	class Program
	{
		static void Main(string[] args)
		{
			if (args == null || args.Length == 0 || ArgumentReader.Exists("h", "help"))
			{
				Console.WriteLine("Simplified User Interface Framework Compiler");
				Console.WriteLine();

				ArgumentPrinter.PrintArgumentUsage("  suifc [path-to-application]");
				ArgumentPrinter.PrintArgumentUsage("  suifc [options] [path-to-application]");
				Console.WriteLine();
				ArgumentPrinter.PrintArgumentExample("suifc \"C:/my_project/input.txt\"");
				ArgumentPrinter.PrintArgumentExample("suifc -l 0 -o \"./input.html\" \"C:/my_project/input.txt\"");

				Console.WriteLine();

				Console.WriteLine("Options:");
				ArgumentPrinter.PrintArgumentDescription(
								"l", "log", "limit",
								$"Sets the amount of information to write to the terminal. Defaults to Info (1), and will print anything equal to or higher than the limit set.\n" +
								$"Choices (number OR name):\n" +
								$"  {(int)LogLevel.trace}   {LogLevel.trace}\n" +
								$"  {(int)LogLevel.info }   {LogLevel.info }\n" +
								$"  {(int)LogLevel.warn }   {LogLevel.warn }\n" +
								$"  {(int)LogLevel.error}   {LogLevel.error}\n");

				ArgumentPrinter.PrintArgumentDescription(
								"o", "output", "path",
								$"Sets the path to the output file to compile to. This can be absolute, or can be relative to the source file. A relative file starts with \"./\".\n" +
								$"Examples:\n" +
								$"  -o \"C:/directory/file.txt\"\n" +
								$"  -o \"./output.html\"\n");

				ArgumentPrinter.PrintArgumentDescription(
								"r", "real-time", null,
								$"Enables realtime compilation. The compiler instance will start running in a loop, watching the target file. " +
								$"Whenever a change is detected the file is recompiled automatically.\n");
			}
			else if (ArgumentReader.Exists("v", "version"))
			{
				var version = typeof(Program).Assembly.GetName().Version;
				Console.WriteLine(version);
			}
			else
			{
				Log.LogLevel = ArgumentReader.Enum("l", "log", LogLevel.info);
				Log.Trace($"Running compiler with log level {Log.LogLevel}");

				var output = ArgumentReader.String("o", "output");
				if(!string.IsNullOrWhiteSpace(output))
					Log.Trace($"Output set to {output}");
				
				var path = ArgumentReader.Last();
				if(string.IsNullOrWhiteSpace(path))
					Log.Fatal("No path set. The last argument should be a path to the file to compile.");

				if(!File.Exists(path))
					Log.Fatal("Invalid path. The last argument should be a path to the file to compile.");
				
				Log.Trace($"Path set to {path}");

				var realtimeMode = ArgumentReader.Exists("r", "real-time");
				if (realtimeMode)
					Log.Trace("Real-time mode enabled");

				var unknownArguments = ArgumentReader.GetUnhandledArguments();
				if(unknownArguments.Length > 0)
				{
					Log.Error($"Unknown {(unknownArguments.Length == 1 ? "argument" : "arguments")}:");
					foreach (var argument in unknownArguments)
						Log.Error($"  {argument}");
					Environment.Exit(1);
				}

				// TODO:: Implement compiler...
			}
		}
	}
}
