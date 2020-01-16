using System.IO;
using System.Reflection;

namespace G1ANT.Addon.JavaUI.Services
{
    public class AssemblyService : IAssemblyService
    {
        public Stream GetManifestResourceStream(Assembly assembly, string resourceFullName) => assembly.GetManifestResourceStream(resourceFullName);

        public Assembly GetExecutingAssembly() => Assembly.GetExecutingAssembly();

        public string[] GetManifestResourceNames(Assembly assembly) => assembly.GetManifestResourceNames();
    }
}
