using G1ANT.Addon.JavaUI.PathParser;
using NUnit.Framework;
using System;

namespace G1ANT.Addon.JavaUI.Tests
{
    [TestFixture]
    public class PathElementTests
    {
        [TestCase("/")]
        [TestCase("/a")]
        [TestCase("/a/b")]
        [TestCase("/*")]
        public void ShouldThrow_WhenPathIsIncorrect(string path)
        {
            Assert.Throws<ArgumentException>(() => new PathElement(path));
        }

        [TestCase("a")]
        [TestCase("1")]
        [TestCase("*")]
        [TestCase("")]
        [TestCase("name=")]
        public void ShouldNotThrow_WhenPathIsCorrect(string path)
        {
            Assert.DoesNotThrow(() => new PathElement(path));
        }

        [Test]
        public void ShouldFillName_WhenNameIsGiven()
        {
            const string elementName = "elementName";
            var result = new PathElement($"name={elementName}");

            Assert.AreEqual(elementName, result.Name);
            Assert.IsNull(result.Index);
            Assert.IsNull(result.Role);
            Assert.IsNull(result.Description);
        }

        [Test]
        public void ShouldFillRole_WhenTypeIsGiven()
        {
            const string typeName = "elementName";
            var result = new PathElement($"type={typeName}");

            Assert.IsNull(result.Name);
            Assert.IsNull(result.Index);
            Assert.AreEqual(typeName, result.Role);
            Assert.IsNull(result.Description);
        }

        [Test]
        public void ShouldFillRole_WhenRoleIsGiven()
        {
            const string typeName = "elementName";
            var result = new PathElement($"role={typeName}");

            Assert.IsNull(result.Name);
            Assert.IsNull(result.Index);
            Assert.AreEqual(typeName, result.Role);
            Assert.IsNull(result.Description);
        }

        [Test]
        public void ShouldSetWildcard_WhenWildcardIsGiven()
        {
            var result = new PathElement(PathParser.PathParser.Wildcard.ToString());

            Assert.IsTrue(result.IsWildcard);
            Assert.IsNull(result.Name);
            Assert.IsNull(result.Index);
            Assert.IsNull(result.Role);
            Assert.IsNull(result.Description);
        }

        [Test]
        public void ShouldFillDescripion_WhenDescriptionIsGiven()
        {
            const string description = "description";
            var result = new PathElement($"description={description}");

            Assert.IsNull(result.Name);
            Assert.IsNull(result.Index);
            Assert.IsNull(result.Role);
            Assert.AreEqual(description, result.Description);
        }


        [Test]
        public void ShouldFillOnlyChildIndex_WhenOnlyIndexIsGiven()
        {
            const int index = 1;
            var result = new PathElement($"[{index}]");

            Assert.IsNull(result.Name);
            Assert.AreEqual(index, result.Index);
            Assert.IsNull(result.Role);
            Assert.IsNull(result.Description);
        }

        [Test]
        public void ShouldFillIndexAndName_WhenIndexAndNameAreGiven()
        {
            const int index = 1;
            const string name = "name";
            var result = new PathElement($"{name}[{index}]");

            Assert.AreEqual(name, result.Name);
            Assert.AreEqual(index, result.Index);
            Assert.IsNull(result.Role);
            Assert.IsNull(result.Description);
        }

        [Test]
        public void ShouldFillIndexAndType_WhenIndexAndTypeAreGiven()
        {
            const int index = 1;
            const string typeName = "typeName";
            var result = new PathElement($"type={typeName}[{index}]");

            Assert.IsNull(result.Name);
            Assert.AreEqual(index, result.Index);
            Assert.AreEqual(typeName, result.Role);
            Assert.IsNull(result.Description);
        }

        [Test]
        public void ShouldSetWildcardAndIndex_WhenWildcardAndIndexAreGiven()
        {
            const int index = 2;
            var result = new PathElement($"{PathParser.PathParser.Wildcard}[{index}]");

            Assert.IsNull(result.Name);
            Assert.IsTrue(result.IsWildcard);
            Assert.AreEqual(index, result.Index);
            Assert.IsNull(result.Role);
            Assert.IsNull(result.Description);
        }

    }
}
