﻿using System;
using System.Collections.Generic;
using WindowsAccessBridgeInterop;

namespace G1ANT.Addon.JavaUI.Services
{
    public class AccessBridgeFactory
    {
        private static readonly AccessBridge accessBridge;

        static AccessBridgeFactory()
        {
            var settingService = new SettingsService();
            new ResourceService().ExtractResources(
                IntPtr.Size == 4 
                    ? new List<string>() { "WindowsAccessBridge-32.dll", "WindowsAccessBridge.dll" }
                    : new List<string>() { "WindowsAccessBridge-64.dll" }
            );

            accessBridge = new AccessBridge(settingService.GetUserDocsAddonFolder());

            // needs some (!) time to run in the background in order to collect all java vms/windows + each instance has to be initialized separately
            accessBridge.Functions.Windows_run();
        }

        public AccessBridge GetAccessBridge() => accessBridge;
    }
}
