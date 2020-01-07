using G1ANT.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using WindowsAccessBridgeInterop;

namespace G1ANT.Addon.JavaUI.Services
{
    public class AccessBridgeFactory : IExtractResources
    {
        private static readonly AccessBridge accessBridge;
        private readonly IFileService fileService;
        private readonly IAssemblyService assemblyService;
        private readonly ISettingsService settingsService;

        static AccessBridgeFactory()
        {
            var settingService = new SettingsService();
            new AccessBridgeFactory(new FileService(), new AssemblyService(), settingService).ExtractResources();
            accessBridge = new AccessBridge(settingService.GetUserDocsAddonFolder());

            // needs some (!) time to run in the background in order to collect all java vms/windows + each instance has to be initialized separately
            accessBridge.Functions.Windows_run();
        }

        public AccessBridgeFactory(IFileService fileService, IAssemblyService assemblyService, ISettingsService settingsService) // IoC
        {
            this.fileService = fileService;
            this.assemblyService = assemblyService;
            this.settingsService = settingsService;
        }


        public AccessBridgeFactory()
            : this(new FileService(), new AssemblyService(), new SettingsService())
        { }

        public AccessBridge GetAccessBridge() => accessBridge;


        public void ExtractResources()
        {
            var currentAssembly = assemblyService.GetExecutingAssembly();
            var resourceFullNames = currentAssembly.GetManifestResourceNames();
            var destinationFolder = settingsService.GetUserDocsAddonFolder();

            var resourceNames = GetResourcesNamesToExtract();

            resourceNames.ForEach(rn => ExtractNewResource(currentAssembly, resourceFullNames, destinationFolder, rn));
        }

        private void ExtractNewResource(Assembly currentAssembly, string[] resourceFullNames, string destinationFolder, string resourceName)
        {
            var resourceFullName = resourceFullNames.Single(e => e.EndsWith(resourceName));
            var resourceStream = assemblyService.GetManifestResourceStream(currentAssembly, resourceFullName);

            if (!fileService.DoesFileExist(destinationFolder, resourceName) || !fileService.AreFilesOfTheSameLength(resourceStream.Length, destinationFolder, resourceName))
            {
                using (var destinationStream = fileService.Create(fileService.Combine(destinationFolder, resourceName)))
                {
                    resourceStream.CopyTo(destinationStream);
                }
            }
        }

        private static List<string> GetResourcesNamesToExtract()
        {
            var resourceNames = new List<string>();
            if (IntPtr.Size == 4)
            {
                resourceNames.Add("WindowsAccessBridge-32.dll");
                resourceNames.Add("WindowsAccessBridge.dll");
            }
            else
                resourceNames.Add("WindowsAccessBridge-64.dll");
            return resourceNames;
        }
    }
}
