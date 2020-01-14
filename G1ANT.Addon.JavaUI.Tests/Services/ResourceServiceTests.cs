using G1ANT.Addon.JavaUI.Services;
using Moq;
using NUnit.Framework;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace G1ANT.Addon.JavaUI.Tests.Services
{
    [TestFixture]
    class ResourceServiceTests
    {
        private Mock<IFileService> fileServiceMock;
        private Mock<IAssemblyService> assemblyServiceMock;
        private Mock<ISettingsService> settingsServiceMock;
        private ResourceService sut;

        [SetUp]
        public void Setup()
        {
            fileServiceMock = new Mock<IFileService>();
            assemblyServiceMock = new Mock<IAssemblyService>();
            settingsServiceMock = new Mock<ISettingsService>();

            sut = new ResourceService(fileServiceMock.Object, assemblyServiceMock.Object, settingsServiceMock.Object);
        }

        [TestFixture]
        public class ExtractResourcesTests : ResourceServiceTests
        {
            [Test]
            public void ShouldStoreResourceInStream_WhenAllDataIsCorrect()
            {
                var assembly = Assembly.GetExecutingAssembly();
                var resourcesToExtract = new string[] { "WindowsAccessBridge.mock" };
                var resourceStreamsFullNames = new string[] { "Resources.WindowsAccessBridge.mock" };
                var userDocsAddonFolder = "userDocsAddonFolder";

                assemblyServiceMock
                    .Setup(ass => ass.GetExecutingAssembly())
                    .Returns(assembly);
                assemblyServiceMock
                    .Setup(ass => ass.GetManifestResourceNames(assembly))
                    .Returns(resourceStreamsFullNames);
                settingsServiceMock
                    .Setup(ss => ss.GetUserDocsAddonFolder())
                    .Returns(userDocsAddonFolder);

                var stringToStore = "stringToStore";
                var bytesToStore = Encoding.UTF8.GetBytes(stringToStore);
                using (var resourceStream = new MemoryStream(bytesToStore))
                {
                    var destinationStreamMock = new Mock<MemoryStream>();
                    destinationStreamMock.Setup(ds => ds.CanWrite).Returns(true);

                    resourceStream.Seek(0, SeekOrigin.Begin);

                    assemblyServiceMock
                        .Setup(ass => ass.GetManifestResourceStream(assembly, resourceStreamsFullNames[0]))
                        .Returns(resourceStream);

                    fileServiceMock
                        .Setup(fs => fs.DoesFileExist(userDocsAddonFolder, resourcesToExtract[0]))
                        .Returns(false);
                    fileServiceMock
                        .Setup(fs => fs.AreFilesOfTheSameLength(resourceStream.Length, userDocsAddonFolder, resourcesToExtract[0]))
                        .Returns(false);
                    var combinedPathToDestinationFile = "combinedPathToDestinationFile";
                    fileServiceMock
                        .Setup(fs => fs.Combine(userDocsAddonFolder, resourcesToExtract[0]))
                        .Returns(combinedPathToDestinationFile);
                    fileServiceMock
                        .Setup(fs => fs.Create(combinedPathToDestinationFile))
                        .Returns(destinationStreamMock.Object);

                    sut.ExtractResources(resourcesToExtract);

                    destinationStreamMock.Verify(
                        ds => ds.Write(It.Is<byte[]>(
                            p => Encoding.UTF8.GetString(p.Take(stringToStore.Length).ToArray()) == stringToStore),
                            0,
                            stringToStore.Length
                        )
                    );
                }
            }
        }

    }
}
