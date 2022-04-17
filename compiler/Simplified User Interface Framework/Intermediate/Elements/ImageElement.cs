using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SimplifiedUserInterfaceFramework.Internal.Reader;

namespace SimplifiedUserInterfaceFramework.Intermediate.Elements
{
	public class ImageElement : Element
	{
		public int? Width { get; private set; }
		public int? Height { get; private set; }
		public string Alt { get; private set; }

		public ImageElement(LineReader reader, Element parent = null) : base(reader, parent, false) { }

		protected override void OnLoad()
		{
			if (Configurations != null)
			{
				if (Configurations.TryGetValue("width", out var width))   Width = int.Parse(width);
				if (Configurations.TryGetValue("height", out var height)) Height = int.Parse(height);
				if (Configurations.TryGetValue("alt", out var alt))       Alt = alt;
			}
		}


		protected override void WriteCoreHtmlDefinition(StreamWriter writer)
		{
			writer.Write($"img src=\"{Value}\"{HtmlFormattedClasses()}");

			if (Width.HasValue)
				writer.Write($" width=\"{Width.Value}\"");

			if (Height.HasValue)
				writer.Write($" height=\"{Height.Value}\"");

			if (!string.IsNullOrWhiteSpace(Alt))
				writer.Write($" alt=\"{Alt}\"");
		}
	}
}
