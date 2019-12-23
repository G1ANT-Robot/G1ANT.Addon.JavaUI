using NUnit.Framework;

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
        public void ShouldReturnSingleElement_WhenPathIsEmpty(string path)
        {
            var result = put.Parse(path);
            Assert.AreEqual(0, result.Count);
        }

        [TestCase("/name")]
        [TestCase("name")]
        public void ShouldReturnSingleElement_WhenPathContainsSingleElement(string path)
        {
            var result = put.Parse(path);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(path.Trim(new char[] { '/' }), result[0].Name);
            Assert.AreEqual(0, result[0].Index);
            Assert.IsNull(result[0].Role);
            Assert.IsNull(result[0].Description);
        }

        [Test]
        public void ShouldReturnTwoElements_WhenPathContainsTwoElements()
        {
            var result = put.Parse("/root/child");

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("root", result[0].Name);
            Assert.AreEqual("child", result[1].Name);
        }

        [Test]
        public void ShouldFillName_WhenNameIsGiven()
        {
            const string elementName = "elementName";
            var result = put.Parse($"/name={elementName}");

            Assert.AreEqual(elementName, result[0].Name);
            Assert.AreEqual(0, result[0].Index);
            Assert.IsNull(result[0].Role);
            Assert.IsNull(result[0].Description);
        }

        [Test]
        public void ShouldFillRole_WhenTypeIsGiven()
        {
            const string typeName = "elementName";
            var result = put.Parse($"/type={typeName}");

            Assert.IsNull(result[0].Name);
            Assert.AreEqual(0, result[0].Index);
            Assert.AreEqual(typeName, result[0].Role);
            Assert.IsNull(result[0].Description);
        }

        [Test]
        public void ShouldFillRole_WhenRoleIsGiven()
        {
            const string typeName = "elementName";
            var result = put.Parse($"/role={typeName}");

            Assert.IsNull(result[0].Name);
            Assert.AreEqual(0, result[0].Index);
            Assert.AreEqual(typeName, result[0].Role);
            Assert.IsNull(result[0].Description);
        }

        [Test]
        public void ShouldFillDescripion_WhenDescriptionIsGiven()
        {
            const string description = "description";
            var result = put.Parse($"/description={description}");

            Assert.IsNull(result[0].Name);
            Assert.AreEqual(0, result[0].Index);
            Assert.IsNull(result[0].Role);
            Assert.AreEqual(description, result[0].Description);
        }


        [Test]
        public void ShouldFillOnlyChildIndex_WhenOnlyIndexIsGiven()
        {
            const int index = 1;
            var result = put.Parse($"/[{index}]");

            Assert.IsNull(result[0].Name);
            Assert.AreEqual(index, result[0].Index);
            Assert.IsNull(result[0].Role);
            Assert.IsNull(result[0].Description);
        }

        [Test]
        public void ShouldFillIndexAndName_WhenIndexAndNameIsGiven()
        {
            const int index = 1;
            const string name = "name";
            var result = put.Parse($"/{name}[{index}]");

            Assert.AreEqual(name, result[0].Name);
            Assert.AreEqual(index, result[0].Index);
            Assert.IsNull(result[0].Role);
            Assert.IsNull(result[0].Description);
        }

        [Test]
        public void ShouldFillIndexAndType_WhenIndexAndTypeIsGiven()
        {
            const int index = 1;
            const string typeName = "typeName";
            var result = put.Parse($"/type={typeName}[{index}]");

            Assert.IsNull(result[0].Name);
            Assert.AreEqual(index, result[0].Index);
            Assert.AreEqual(typeName, result[0].Role);
            Assert.IsNull(result[0].Description);
        }
    }
}
