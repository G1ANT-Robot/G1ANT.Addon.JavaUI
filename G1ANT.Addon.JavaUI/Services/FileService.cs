using System.IO;

namespace G1ANT.Addon.JavaUI.Services
{
    public class FileService : IFileService
    {
        public bool DoesFileExist(string folder, string fileName)
        {
            return File.Exists(Path.Combine(folder, fileName));
        }

        public bool AreFilesOfTheSameLength(long length, string folder, string fileName)
        {
            return length == new FileInfo(Path.Combine(folder, fileName)).Length;
        }

        public Stream Create(string path)
        {
            return File.Create(path);
        }

        public string Combine(params string[] path)
        {
            return Path.Combine(path);
        }
    }
}
