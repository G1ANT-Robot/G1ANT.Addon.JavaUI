using System.IO;

namespace G1ANT.Addon.JavaUI.Services
{
    public interface IFileService
    {
        bool DoesFileExist(string folder, string fileName);
        bool AreFilesOfTheSameLength(long length, string folder, string fileName);
        Stream Create(string path);
        string Combine(params string[] path);
    }
}