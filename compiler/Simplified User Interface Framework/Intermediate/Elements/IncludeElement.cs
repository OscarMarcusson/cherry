﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SimplifiedUserInterfaceFramework.Internal.Reader;

namespace SimplifiedUserInterfaceFramework.Intermediate.Elements
{
	public class IncludeElement : Element
	{
		public string FullPath { get; set; }

		public IncludeElement(LineReader reader, Element parent = null) : base(reader, parent, false)
		{
		}



		protected override void OnLoad()
		{
			if (Value == null)
				throw new SectionException(Source.Text, " ", "", "Expected a path value", Source.LineNumber);

			var extension = Path.GetExtension(Value);
			if (extension != ".md" && extension != ".markdown")
				throw new SectionException(Source.Text.Substring(0, Source.Text.Length - extension.Length + 1), extension?.Substring(1) ?? " ", "", "Expected .md or .markdown", Source.LineNumber);

			var root = "..";	// TODO:: Resolve this, some simple object thats sent into the load argument perhaps?
			FullPath = Path.Combine(root, Value);
			if (!File.Exists(FullPath))
			{
				var i = Source.Text.LastIndexOf('=') + 1;
				for (; i < Source.Text.Length; i++)
				{
					if (Source.Text[i] != ' ' && Source.Text[i] != '\t')
						break;
				}

				var left = Source.Text.Substring(0, i);
				var value = Source.Text.Substring(i);
				throw new SectionException(left, value, "", "Could not find this file", Source.LineNumber);
			}
		}
	}
}
