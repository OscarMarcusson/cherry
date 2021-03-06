using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Cherry.Internal.Reader;

namespace Cherry.Intermediate.Elements
{
	public class IframeElement : Element
	{
		public string Width { get; private set; }
		public string Height { get; private set; }
		public string Default { get; private set; }

		public IframeElement(VariablesCache parentVariables, LineReader reader, Element parent, CompilerArguments compilerArguments) : base(parentVariables, reader, parent, false, compilerArguments)
		{
		}

		protected override void OnLoad()
		{
			Name = "iframe";
			Type = ElementType.IFrame;

			if (Configurations != null)
			{
				if (Configurations.TryGetValue("width", out var width)) Width = width;
				if (Configurations.TryGetValue("height", out var height)) Height = height;
				if (Configurations.TryGetValue("default", out var alt)) Default = alt;
			}

			if (string.IsNullOrWhiteSpace(Width)) Width = "100%";
			if (string.IsNullOrWhiteSpace(Height)) Height = "100%";
		}


		protected override string HtmlTag => "iframe";

		protected override void WriteCoreHtmlDefinition(StreamWriter writer)
		{
			writer.Write($"{HtmlTag} src=\"{Value}\"{HtmlFormattedClasses()}");

			writer.Write($" width=\"{Width}\"");
			writer.Write($" height=\"{Height}\"");
		}

		public override void ValueToHtml(StreamWriter writer)
		{
			if(!string.IsNullOrWhiteSpace(Default))
				writer.Write(Default);
		}

		protected override bool WriteValueAutomatically => true;
	}
}
