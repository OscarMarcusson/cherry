using System;
using SimplifiedUserInterfaceFramework.Internal;

namespace SimplifiedUserInterfaceFramework
{
	class Program
	{
		static void Main(string[] args)
		{
			if (args == null || args.Length == 0)
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
								$"  {(int)LogLevel.Trace}   {LogLevel.Trace}\n" +
								$"  {(int)LogLevel.Info }   {LogLevel.Info }\n" +
								$"  {(int)LogLevel.Warn }   {LogLevel.Warn }\n" +
								$"  {(int)LogLevel.Error}   {LogLevel.Error}\n");

				ArgumentPrinter.PrintArgumentDescription(
								"o", "output", "path",
								$"Sets the path to the output file to compile to. This can be absolute, or can be relative to the source file. A relative file starts with \"./\".\n" +
								$"Examples:\n" +
								$"  -o \"C:/directory/file.txt\"\n" +
								$"  -o \"./output.html\"\n");

				ArgumentPrinter.PrintArgumentDescription(
								"r", "realtime", null,
								$"Enables realtime compilation. The compiler instance will start running in a loop, watching the target file. " +
								$"Whenever a change is detected the file is recompiled automatically.\n");
			}
			else
			{
				Log.LogLevel = ArgumentReader.Enum("l", "log", LogLevel.Info);
				Log.Trace($"Running compiler with log level {Log.LogLevel}");

				var output = ArgumentReader.String("o", "output");
				if(!string.IsNullOrWhiteSpace(output))
					Log.Trace($"Output set to {output}");
				
				var path = ArgumentReader.Last();
				if(string.IsNullOrWhiteSpace(path))
					Log.Fatal("No path set. The last argument should be a path to the file to compile.");

				Log.Trace($"Path set to {path}");

				// TODO:: Implement compiler...
			}
		}
	}
}
