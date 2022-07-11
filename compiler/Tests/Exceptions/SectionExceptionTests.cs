using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cherry;

namespace Exceptions
{
    [TestClass]
    public class SectionExceptionTests
    {
        [TestMethod]
        public void CanHandleEmptyInput()
        {
            var exception = new SectionException(null, null, null, "Error",123, "TestFile.txt");
            Assert.AreEqual("", exception.Left);
            Assert.AreEqual("", exception.Center);
            Assert.AreEqual("", exception.Right);
            exception = new SectionException("", "", "", "Error",123, "TestFile.txt");
            Assert.AreEqual("", exception.Left);
            Assert.AreEqual("", exception.Center);
            Assert.AreEqual("", exception.Right);
        }
        

        [TestMethod]
        public void ThrowsArgumentExceptionForMissingErrorMessage()
        {
            Assert.ThrowsException<ArgumentException>(() => new SectionException("left", "center", "right", ""));
            Assert.ThrowsException<ArgumentException>(() => new SectionException("left", "center", "right", "   \t  "));
            Assert.ThrowsException<ArgumentException>(() => new SectionException("left", "center", "right", null));
        }
        
        
        [TestMethod]
        public void SavesError()
        {
            var exception = new SectionException("a", "b", "c", "Error msg");
            Assert.AreEqual("Error msg", exception.Message);
        }
        
        
        [TestMethod]
        public void SavesFileInformation()
        {
            var exception = new SectionException("a", "b", "c", "Error", fileName:"TestFile.txt");
            Assert.AreEqual("TestFile.txt", exception.FileName);
        }
        
        
        [TestMethod]
        public void SavesLineInformation()
        {
            var exception = new SectionException("a", "b", "c", "Error", lineNumber:123);
            Assert.AreEqual(123, exception.LineNumber);
        }
        
        
        [TestMethod]
        public void HasCorrectSingleLineIndentation()
        {
            var left = "Place error ";
            var exception = new SectionException(left, "here", "", "There it is!");
            Assert.AreEqual(left.Length, exception.Indentation);
        }
        
        
        [TestMethod]
        public void HasCorrectMultiLineIndentation()
        {
            var lineToPlaceAfter = "Place error ";
            var left = "Ignore\nthese\nlong lines here, we only want the next one:\n" + lineToPlaceAfter;
            var exception = new SectionException(left, "here", "", "There it is!");
            Assert.AreEqual(lineToPlaceAfter.Length, exception.Indentation);
        }

        
        [TestMethod]
        public void SectionArrowsHaveCorrectLength()
        {
            var center = "I am the center";
            var e = new SectionException("left", center, "right", "Error");
            Assert.AreEqual(center.Length, e.GetSectionArrows.Length);
        }
        
        
        [TestMethod]
        public void SectionArrowsIsAtLeast1Long()
        {
            var exception = new SectionException("we are missing some word at the end", "", "", "Expected \"here\" here");
            Assert.AreEqual(1, exception.GetSectionArrows.Length);
        }
    }
}