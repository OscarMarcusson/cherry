using System;

namespace Cherry
{
	public class SectionException : Exception
	{
		public readonly string Left;
		public readonly string Center;
		public readonly string Right;
		public readonly string FileName;
		public readonly int LineNumber;
		public readonly int Indentation;

		public SectionException(string line, int index, int count, string error, int lineNumber = -1, string fileName = null) 
			: this(
				  left: index > 0
							? line.Substring(index, count)
							: ""
							,
				  center: line.Substring(index, count),
				  right: index + count < line.Length
							? line.Substring(index+count)
							: ""
							,
				  error: error,
				  lineNumber: lineNumber,
				  fileName: fileName
				  )
			{ }

		public SectionException(string left, string center, string right, string error, int lineNumber = -1, string fileName = null) : base(error ?? "")
		{
			if (string.IsNullOrWhiteSpace(error))
				throw new ArgumentException($"Can't create a section exception with an empty error.\nInput:\nLeft = {left}\nCenter = {center}\nRight = {right}\nLine number = {lineNumber}\nFile name = {fileName}");
			
			Left = left ?? "";
			Center = center ?? "";
			Right = right ?? "";
			FileName = fileName;
			LineNumber = lineNumber;

			var index = Left.LastIndexOf('\n');
			Indentation = index > 0 ? Left.Length-index-1 : Left.Length;
		}

		public string GetSectionArrows => Center.Length == 0 ? "^" : new string('^', Center.Length);
	}
}