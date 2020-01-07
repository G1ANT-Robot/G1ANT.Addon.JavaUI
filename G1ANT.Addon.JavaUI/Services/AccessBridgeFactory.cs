using G1ANT.Language;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using WindowsAccessBridgeInterop;

namespace G1ANT.Addon.JavaUI.Services
{
    public class AccessBridgeFactory
    {
        private static readonly AccessBridge accessBridge;

        static AccessBridgeFactory()
        {
            UnpackLibraries();

            accessBridge = new AccessBridge(AbstractSettingsContainer.Instance.UserDocsAddonFolder.FullName);
            // needs some (!) time to run in the background in order to collect all java vms/windows + each instance has to be initialized separately
            accessBridge.Functions.Windows_run();
        }

        public AccessBridge GetAccessBridge() => accessBridge;


        private static void UnpackLibraries()
        {
            var currentAssembly = Assembly.GetExecutingAssembly();
            var resourceFullNames = currentAssembly.GetManifestResourceNames();
            var unpackFolder = AbstractSettingsContainer.Instance.UserDocsAddonFolder.FullName;


            var resourceNames = new List<string>();
            if (IntPtr.Size == 4)
            {
                resourceNames.Add("WindowsAccessBridge-32.dll");
                resourceNames.Add("WindowsAccessBridge.dll");
            }
            else
                resourceNames.Add("WindowsAccessBridge-64.dll");

            foreach (var resourceName in resourceNames)
            {
                var resourceFullName = resourceFullNames.Single(e => e.EndsWith(resourceName));
                var resourceStream = currentAssembly.GetManifestResourceStream(resourceFullName);

                if (!DoesFileExist(unpackFolder, resourceName) || !AreFilesOfTheSameLength(resourceStream.Length, unpackFolder, resourceName))
                {
                    using (var destinationStream = File.Create(Path.Combine(unpackFolder, resourceName)))
                    {
                        resourceStream.CopyTo(destinationStream);
                    }
                }
            }
        }

        private static bool DoesFileExist(string folder, string fileName)
        {
            return File.Exists(Path.Combine(folder, fileName));
        }

        private static bool AreFilesOfTheSameLength(long length, string folder, string fileName)
        {
            return length == new FileInfo(Path.Combine(folder, fileName)).Length;
        }

    }
}
