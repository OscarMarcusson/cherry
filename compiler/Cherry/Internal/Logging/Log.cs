using System;

namespace Cherry.Internal
{
    public class Log
    {
        private readonly object LogLevelLocker = new object();
        LogLevel logLevel;

        public LogLevel LogLevel
        {
            get
            {
                lock (LogLevelLocker)
                    return logLevel;
            }
            set
            {
                lock (LogLevelLocker)
                    logLevel = value;
            }
        }

        public void Trace(string message) => LogMessage(LogLevel.Trace, message, ConsoleColor.DarkGray);
        public void Info(string message) => LogMessage(LogLevel.Info, message, ConsoleColor.Gray);
        public void Warning(string message) => LogMessage(LogLevel.Warn, message, ConsoleColor.Yellow);

        public void Error(string message) => LogMessage(LogLevel.Error, message, ConsoleColor.Red);

        public void Fatal(string message)
        {
            LogMessage(LogLevel.Fatal, message, ConsoleColor.Red);
            throw new ApplicationException(message);
        }
        
        void LogMessage(LogLevel level, string msg, ConsoleColor color)
        {
            lock (LogLevelLocker)
            {
                if (level >= logLevel)
                {
                    Console.ForegroundColor = color;
                    Console.WriteLine(msg);
                }    
            }
        }

        public void SectionError(SectionException e)
        {
            lock (LogLevelLocker)
            {

                var indent = e.Indentation > 0 ? new string(' ', e.Indentation) : "";
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine($"Failed to parse {e.FileName}{(e.LineNumber > -1 ? $"\nLine {e.LineNumber}" : "")}");
                Console.ResetColor();
                Console.Write(e.Left);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(e.Center);
                Console.ResetColor();
                Console.WriteLine(e.Right);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(indent + e.GetSectionArrows);
                Console.WriteLine(indent + e.Message);
                Console.ResetColor();
            }
        }
    }
}