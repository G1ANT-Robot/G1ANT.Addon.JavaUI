using WindowsAccessBridgeInterop;

namespace G1ANT.Addon.JavaUI.Services
{
    public class AccessBridgeFactory
    {
        private static AccessBridge accessBridge = new AccessBridge();

        static AccessBridgeFactory()
        {
            // needs some (!) time to run in the background in order to collect all java vms/windows + each instance has to be initialized separately
            accessBridge.Functions.Windows_run();
        }

        public AccessBridge GetAccessBridge() => accessBridge;
    }
}
