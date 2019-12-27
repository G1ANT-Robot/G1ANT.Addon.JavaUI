using System;
using System.Collections.Generic;
using System.Linq;
using WindowsAccessBridgeInterop;

namespace G1ANT.Addon.JavaUI.PathParser
{
    public interface INodeService
    {
        IReadOnlyCollection<AccessibleJvm> GetJvms();
        IReadOnlyCollection<NodeModel> GetJvmNodes();

        IReadOnlyCollection<AccessibleNode> GetChildren(AccessibleNode node);
        IReadOnlyCollection<NodeModel> GetChildNodes(NodeModel node);

        AccessibleContextInfo GetNodeInfo(AccessibleNode node);
    }

    public class NodeService : INodeService
    {
        private readonly AccessBridge accessBridge;
        private static bool initialized = false;
        private static object initializationLock = new object();

        static NodeService()
        {
            new AccessBridge().Functions.Windows_run();
        }

        public NodeService(AccessBridge accessBridge)
        {
            this.accessBridge = accessBridge;
        }

        public IReadOnlyCollection<AccessibleJvm> GetJvms()
        {
            return accessBridge.EnumJvms(hwnd => accessBridge.CreateAccessibleWindow(hwnd));
        }

        public IReadOnlyCollection<NodeModel> GetJvmNodes()
        {
            return GetJvms()
                .Select(jvm => new NodeModel(jvm))
                .ToList();
        }

        public IReadOnlyCollection<NodeModel> GetChildNodes(NodeModel node)
        {
            return GetChildren(node.Node)
                .Select(ch => new NodeModel(ch))
                .ToList();
        }

        public IReadOnlyCollection<AccessibleNode> GetChildren(AccessibleNode node)
        {
            return node.GetChildren().ToList();
        }


        public AccessibleContextInfo GetNodeInfo(AccessibleNode node)
        {
            if (node is AccessibleWindow accessibleWindow)
                return accessibleWindow.GetInfo();

            if (node is AccessibleContextNode accessibleContextNode)
                return accessibleContextNode.GetInfo();

            throw new Exception($"Unsupported type of node: {node.GetType()}");
        }
    }
}
