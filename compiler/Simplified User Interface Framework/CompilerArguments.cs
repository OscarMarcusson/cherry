using System;
using System.Collections.Generic;
using System.Text;

namespace SimplifiedUserInterfaceFramework
{
	public class CompilerArguments
	{
		public string Output { get; set; }
		public string Input { get; set; }
		public LogLevel LogLevel { get; set; } = LogLevel.Info;
		public bool RealTime { get; set; }


		public CompilerArguments CreateCopy() => MemberwiseClone() as CompilerArguments;
	}
}
