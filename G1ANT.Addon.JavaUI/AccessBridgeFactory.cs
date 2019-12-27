using WindowsAccessBridgeInterop;

namespace G1ANT.Addon.JavaUI
{
    public class AccessBridgeFactory
    {
        private static AccessBridge accessBridge = new AccessBridge();

        static AccessBridgeFactory()
        {
            accessBridge.Functions.Windows_run(); // it's a shit because it needs some (!) time to run in the background in order to collect all java vms/windows
        }

        public AccessBridge GetAccessBridge()
        {
            return accessBridge;
        }
    }
}
