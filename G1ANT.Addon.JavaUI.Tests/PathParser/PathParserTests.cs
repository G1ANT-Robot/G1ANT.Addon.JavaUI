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

        [Test]
        public void ShouldFillName_WhenNameIsGiven()
        {
            const string elementName = "elementName";
            var result = put.Parse($"/*/name={elementName}");

            Assert.AreEqual(elementName, result[1].Name);
            Assert.IsNull(result[1].Index);
            Assert.IsNull(result[1].Role);
            Assert.IsNull(result[1].Description);
        }

        [Test]
        public void ShouldFillRole_WhenTypeIsGiven()
        {
            const string typeName = "elementName";
            var result = put.Parse($"/*/type={typeName}");

            Assert.IsNull(result[1].Name);
            Assert.IsNull(result[1].Index);
            Assert.AreEqual(typeName, result[1].Role);
            Assert.IsNull(result[1].Description);
        }

        [Test]
        public void ShouldFillRole_WhenRoleIsGiven()
        {
            const string typeName = "elementName";
            var result = put.Parse($"/*/role={typeName}");

            Assert.IsNull(result[1].Name);
            Assert.IsNull(result[1].Index);
            Assert.AreEqual(typeName, result[1].Role);
            Assert.IsNull(result[1].Description);
        }

        [Test]
        public void ShouldFillDescripion_WhenDescriptionIsGiven()
        {
            const string description = "description";
            var result = put.Parse($"/*/description={description}");

            Assert.IsNull(result[1].Name);
            Assert.IsNull(result[1].Index);
            Assert.IsNull(result[1].Role);
            Assert.AreEqual(description, result[1].Description);
        }


        [Test]
        public void ShouldFillOnlyChildIndex_WhenOnlyIndexIsGiven()
        {
            const int index = 1;
            var result = put.Parse($"/*/[{index}]");

            Assert.IsNull(result[1].Name);
            Assert.AreEqual(index, result[1].Index);
            Assert.IsNull(result[1].Role);
            Assert.IsNull(result[1].Description);
        }

        [Test]
        public void ShouldFillIndexAndName_WhenIndexAndNameIsGiven()
        {
            const int index = 1;
            const string name = "name";
            var result = put.Parse($"/*/{name}[{index}]");

            Assert.AreEqual(name, result[1].Name);
            Assert.AreEqual(index, result[1].Index);
            Assert.IsNull(result[1].Role);
            Assert.IsNull(result[1].Description);
        }

        [Test]
        public void ShouldFillIndexAndType_WhenIndexAndTypeIsGiven()
        {
            const int index = 1;
            const string typeName = "typeName";
            var result = put.Parse($"/*/type={typeName}[{index}]");

            Assert.IsNull(result[1].Name);
            Assert.AreEqual(index, result[1].Index);
            Assert.AreEqual(typeName, result[1].Role);
            Assert.IsNull(result[1].Description);
        }
    }
}
