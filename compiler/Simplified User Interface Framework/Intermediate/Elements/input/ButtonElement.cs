﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SimplifiedUserInterfaceFramework.Internal.Reader;

namespace SimplifiedUserInterfaceFramework.Intermediate.Elements
{
	public class ButtonElement : InputElementBase
	{
		public ButtonElement(LineReader reader, Element parent, CompilerArguments compilerArguments) : base(reader, parent, compilerArguments)
		{
		}


		protected override ElementType GetType => ElementType.Button;
		protected override string GetTypeString => "button";
	}
}
