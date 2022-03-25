using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimplifiedUserInterfaceFramework.Internal
{
	// A simple thread safe logger to make it easier to print info to the screen
	internal static class Log
	{
		static readonly object Locker = new object();
		

		static LogLevel logLevel;
		public static LogLevel LogLevel
		{
			get
			{
				lock (Locker)
					return logLevel;
			}
			set
			{
				lock (Locker)
					logLevel = value;
			}
		}


		public static void Trace   (object message) => Message(message, ConsoleColor.DarkGray, LogLevel.trace);
		public static void Info    (object message) => Message(message, ConsoleColor.Gray,     LogLevel.info);
		public static void Warning (object message) => Message(message, ConsoleColor.Yellow,   LogLevel.warn);
		public static void Error   (object message) => Message(message, ConsoleColor.Red,      LogLevel.error);

		public static void Fatal (object message)
		{
			Message(message, ConsoleColor.Red, LogLevel.error);
			Environment.Exit(1);
		}

		static void Message(object rawMessage, ConsoleColor color, LogLevel minimumNeededLogLevel)
		{
			var message = rawMessage?.ToString();
			var isEmpty = string.IsNullOrWhiteSpace(message);

			lock (Locker)
			{
				if (isEmpty | logLevel > minimumNeededLogLevel)
				{
					Console.WriteLine();
				}
				else
				{
					var previousColor = Console.ForegroundColor;
					Console.ForegroundColor = color;
					Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message.Replace("\n", "\n                   ")}");
					Console.ForegroundColor = previousColor;
				}
			}
		}
	}
}
