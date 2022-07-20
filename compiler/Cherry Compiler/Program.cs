using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Cherry.Internal;

namespace Cherry
{
	class Program
	{
		static bool loopThread;
		static AutoResetEvent exitLoopThread = new AutoResetEvent(false);
		static Log log;

		static void Main(string[] args)
		{
			var argumentReader = new ArgumentReader(args, exitOnFatal:true);

			if (args == null || args.Length == 0 || argumentReader.Exists("h", "help"))
			{
				Console.WriteLine("Cherry Compiler");
				Console.WriteLine();

				ArgumentPrinter.PrintArgumentUsage("  cherry [path-to-application]");
				ArgumentPrinter.PrintArgumentUsage("  cherry [options] [path-to-application]");
				Console.WriteLine();
				ArgumentPrinter.PrintArgumentExample("cherry \"C:/my_project/input.txt\"");
				ArgumentPrinter.PrintArgumentExample("cherry -l 0 -o \"./input.html\" \"C:/my_project/input.txt\"");

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

				ArgumentPrinter.PrintArgumentDescription(
								"n", "native", "OS",
								$"Creates a native executable for the given operating system instead of HTML.\n" +
								$"Options:\n" +
								$"  * win-x86\n" +
								$"  * win-x64\n" +
								$"  * linux-x64\n" +
								$"  * linux-arm64\n" +
								$"Examples:\n" +
								$"  -n win-x64\n" +
								$"  -n linux-arm64\n");
			}
			else if (argumentReader.Exists("v", "version"))
			{
				var version = typeof(Program).Assembly.GetName().Version;
				Console.WriteLine(version);
			}
			else
			{
				var arguments = ReadArguments(argumentReader, out log);
				var compiler = new Compiler(arguments);

				if (arguments.RealTime)
				{
					loopThread = true;
					while (loopThread)
					{
						try
						{
							Console.Clear();
							var thread = new Thread(() => CompileThread(compiler));
							thread.Start();

							while (loopThread)
							{
								var input = Console.ReadKey(true);
								switch (input.Key)
								{
									case ConsoleKey.Escape:
										loopThread = false;
										exitLoopThread.Set();
										thread.Join(1000);
										return;
								}
							}
						}
						catch (Exception e)
						{
							Console.ForegroundColor = ConsoleColor.Red;
							Console.WriteLine(e);
							Thread.Sleep(10000);
						}
					}
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

			arguments.RootDirectory = Path.GetDirectoryName(arguments.Input);

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



		static void CompileThread(Compiler compiler)
		{
			var watchers = new List<FileSystemWatcher>();
			while (loopThread)
			{
				DisposeWatchers();

				try
				{
					Compile();

					var allFiles = compiler.GetDocuments();
					foreach(var document in allFiles)
					{
						var filesToWatch = new List<string> { document.Source.File };
						if(document.IncludesScripts?.Length > 0) filesToWatch.AddRange(document.IncludesScripts.Select(x => Path.Combine(compiler.InputDirectory, x.Value)));
						if(document.IncludeStyles?.Length > 0)   filesToWatch.AddRange(document.IncludeStyles.Select(x => Path.Combine(compiler.InputDirectory, x.Value)));

						foreach(var file in filesToWatch)
						{
							if (File.Exists(file))
							{
								var directory = Path.GetDirectoryName(file);
								var fileName = Path.GetFileName(file);
								var watcher = new FileSystemWatcher(directory, fileName);
								watcher.EnableRaisingEvents = true;
								watcher.Changed += (object sender, FileSystemEventArgs e) =>
								{
									if(e.ChangeType == WatcherChangeTypes.Created || e.ChangeType == WatcherChangeTypes.Changed)
									{
										Compile();
									}
								};
								watchers.Add(watcher);
							}
							else
							{
								log.Warning("Could not find " + file);
							}
						}
					}

					exitLoopThread.WaitOne();
				}
				catch (Exception exc)
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine(exc);
					Thread.Sleep(10000);
				}
			}

			// Make sure to dispose before leaving thread
			DisposeWatchers();

			void DisposeWatchers()
			{
				foreach (var watcher in watchers)
					watcher?.Dispose();
				watchers.Clear();
			}

			void Compile()
			{
				Console.Clear();
				compiler.Compile();
				Console.WriteLine();
				Console.WriteLine("Press ESC to exit live mode");
			}
		}
	}
}
