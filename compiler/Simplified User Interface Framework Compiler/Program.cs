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
			var argumentReader = new ArgumentReader(args, exitOnFatal:true);

			if (args == null || args.Length == 0 || argumentReader.Exists("h", "help"))
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
								"r", "real-time", null,
								$"Enables realtime compilation. The compiler instance will start running in a loop, watching the target file. " +
								$"Whenever a change is detected the file is recompiled automatically.\n");
			}
			else if (argumentReader.Exists("v", "version"))
			{
				var version = typeof(Program).Assembly.GetName().Version;
				Console.WriteLine(version);
			}
			else
			{
				var arguments = ReadArguments(argumentReader, out var log);
				var compiler = new Compiler(arguments);

				if (arguments.RealTime)
				{
					log.Fatal("Real-time compilation is not implemented yet...");
				}
				else
				{
					compiler.Compile();
				}
			}
		}


		private static CompilerArguments ReadArguments(ArgumentReader argumentReader, out Log log)
		{
			var arguments = new CompilerArguments();
			arguments.LogLevel = argumentReader.Enum("l", "log", LogLevel.Info);
			log = new Log { LogLevel = arguments.LogLevel };
			argumentReader.Log = log;

			log.Trace($"Running compiler with log level {log.LogLevel}");

			arguments.Output = argumentReader.String("o", "output");
			if (!string.IsNullOrWhiteSpace(arguments.Output))
				log.Trace($"Output set to {arguments.Output}");

			arguments.Input = argumentReader.Last();
			if (string.IsNullOrWhiteSpace(arguments.Input))
				log.Fatal("No path set. The last argument should be a path to the file to compile.");

			if (!File.Exists(arguments.Input))
				log.Fatal("Invalid path. The last argument should be a path to the file to compile.");

			log.Trace($"Path set to {arguments.Input}");

			arguments.RealTime = argumentReader.Exists("r", "real-time");
			if (arguments.RealTime)
				log.Trace("Real-time mode enabled");

			var unknownArguments = argumentReader.GetUnhandledArguments();
			if (unknownArguments.Length > 0)
			{
				log.Error($"Unknown {(unknownArguments.Length == 1 ? "argument" : "arguments")}:");
				foreach (var argument in unknownArguments)
					log.Error($"  {argument}");
				Environment.Exit(1);
			}


			return arguments;
		}
	}
}
