using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace G1ANT.Addon.JavaUI.Services
{
    public class ResourceService : IResourceService
    {
        private IFileService fileService;
        private IAssemblyService assemblyService;
        private ISettingsService settingsService;

        public ResourceService(IFileService fileService, IAssemblyService assemblyService, ISettingsService settingsService) // IoC
        {
            this.fileService = fileService;
            this.assemblyService = assemblyService;
            this.settingsService = settingsService;
        }

        public ResourceService()
          : this(new FileService(), new AssemblyService(), new SettingsService())
        { }

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
