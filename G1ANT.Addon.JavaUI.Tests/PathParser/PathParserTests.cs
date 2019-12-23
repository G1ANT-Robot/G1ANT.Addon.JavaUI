using NUnit.Framework;
using System;

namespace G1ANT.Addon.JavaUI.Tests
{
    [TestFixture]
    public class PathParserTests
    {
        PathParser.PathParser put;

        [SetUp]
        public void Setup()
        {
            put = new PathParser.PathParser();
        }


        [TestCase("")]
        [TestCase("/")]
        [TestCase("/a")]
        [TestCase("/a/b")]
        [TestCase("/a/b/c")]
        [TestCase("/[1]/b/c")]
        [TestCase("/1")]
        [TestCase("///")]
        [TestCase("/*")]
        [TestCase("*")]
        [TestCase("/*/")]
        public void ShouldThrow_WhenPathIsIncorrect(string path)
        {
            Assert.Throws<ArgumentException>(() => put.Parse(path));
        }

        [TestCase("/*/a")]
        [TestCase("/*/1")]
        [TestCase("/*/*")]
        [TestCase("/1/2")]
        [TestCase("/1/2/*/")]
        [TestCase("/1/2/*[1]/")]
        public void ShouldNotThrow_WhenPathIsCorrect(string path)
        {
            Assert.DoesNotThrow(() => put.Parse(path));
        }

        [Test]
        public void ShouldReturnTwoElements_WhenPathContainsTwoElements()
        {
            var result = put.Parse("/1/child");

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(1, result[0].Id);
            Assert.AreEqual("child", result[1].Name);
        }
    }
}
