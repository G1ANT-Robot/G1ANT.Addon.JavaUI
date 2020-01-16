using System.IO;
using System.Reflection;

namespace G1ANT.Addon.JavaUI.Services
{
    public interface IAssemblyService
    {
        Stream GetManifestResourceStream(Assembly assembly, string resourceFullName);

        Assembly GetExecutingAssembly();

        string[] GetManifestResourceNames(Assembly assembly);
    }
}