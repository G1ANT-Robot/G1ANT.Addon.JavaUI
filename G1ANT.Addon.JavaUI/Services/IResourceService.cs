using System.Collections.Generic;

namespace G1ANT.Addon.JavaUI.Services
{
    public interface IResourceService
    {
        void ExtractResources(IEnumerable<string> resourceNames);
    }
}