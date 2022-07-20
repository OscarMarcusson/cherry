using System;
using System.Collections.Generic;
using System.Text;

namespace Cherry
{
	public class CompilerArguments
	{
		public string Output { get; set; }
		public string Input { get; set; }
		public string RootDirectory { get; set; }
		public LogLevel LogLevel { get; set; } = LogLevel.Info;
		public bool RealTime { get; set; }
		public bool IsTest { get; set; }
		public string Native { get; set; }


		public CompilerArguments CreateCopy() => MemberwiseClone() as CompilerArguments;



		public static CompilerArguments Test => new CompilerArguments { IsTest = true };
	}
}
