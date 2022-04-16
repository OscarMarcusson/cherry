using SimplifiedUserInterfaceFramework.Internal.Reader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimplifiedUserInterfaceFramework.Intermediate
{
	public class Style
	{
		public readonly string Name;
		public readonly bool IsGlobal;
		public readonly Dictionary<string, Element> Elements = new Dictionary<string, Element>();



		public Style()
		{
			Name = "Global";
			IsGlobal = true;
		}

		public Style(LineReader reader)
		{
			if(reader.Text != reader.First)
			{
				Name = reader.Text.Substring(reader.First.Length).TrimStart();
				IsGlobal = false;
			}
			else
			{
				Name = "Global";
				IsGlobal = true;
			}

			foreach(var elementReader in reader.Children)
			{
				if(elementReader.Children.Count > 0)
				{
					var elementName = elementReader.Text;

					// Some types should be hard replaced to make it easier to write
					switch (elementName)
					{
						case "tab-selector":
						case ".tab-selector":
						case "ts":
						case ".ts":
							elementName = "input[type=button].tab-selector";
							break;

						case ".tab-content":
						case "tc":
						case ".tc":
							elementName = "tab-content";
							break;
					}

					if (!Elements.TryGetValue(elementName, out var element))
						Elements[elementName] = element = new Element();

					foreach(var valueReader in elementReader.Children)
					{
						var index = valueReader.Text.IndexOf('=');
						if(index > -1)
						{
							var valueName = valueReader.Text.Substring(0, index).TrimEnd().Replace(" ", "");
							var value = valueReader.Text.Substring(index + 1).TrimStart();
							element.Values[valueName] = value;
						}
					}
				}
			}
		}

		public void ReadFrom(Style style)
		{
			foreach(var element in style.Elements)
			{
				if (!Elements.TryGetValue(element.Key, out var existingElement))
					Elements[element.Key] = existingElement = new Element();

				existingElement.ReadFrom(element.Value);
			}
		}





		public class Element
		{
			public readonly Dictionary<string, string> Values = new Dictionary<string, string>();



			public void ReadFrom(Element element)
			{
				foreach(var value in element.Values)
					Values[value.Key] = value.Value;
			}
		}
	}
}
