using System.Collections.Generic;
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
}
