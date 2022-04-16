﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace SimplifiedUserInterfaceFramework
{
	static class CompilerResources
	{
		static readonly Assembly Assembly = typeof(CompilerResources).Assembly;
		const string Prefix = "SimplifiedUserInterfaceFramework.Resources";

		public static string GetJavascript(string name) => Get("Javascript", name, ".js");

		static string Get(string folder, string file, string extension)
		{
			file = Path.ChangeExtension(file, extension);
			var path = string.Join(".", Prefix, folder, file);

			using (Stream stream = Assembly.GetManifestResourceStream(path))
			using (StreamReader reader = new StreamReader(stream))
			{
				string result = reader.ReadToEnd();
				return result;
			}
		}
	}
}
