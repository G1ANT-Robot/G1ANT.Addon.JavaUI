using G1ANT.Addon.JavaUI.Models;
using System.Collections.Generic;
using WindowsAccessBridgeInterop;

namespace G1ANT.Addon.JavaUI.Services
{
    public interface INodeService
    {
        IReadOnlyCollection<AccessibleJvm> GetJvms();
        IReadOnlyCollection<NodeModel> GetJvmNodes();

        IReadOnlyCollection<AccessibleNode> GetChildren(AccessibleNode node);
        IReadOnlyCollection<NodeModel> GetChildNodes(NodeModel node);

        AccessibleContextInfo GetNodeInfo(AccessibleNode node);

        void SetTextContents(AccessibleNode node, string text);
        void RequestFocus(AccessibleNode node);

        IEnumerable<string> GetActions(AccessibleContextNode node);
        void DoAction(AccessibleNode node, string action);
    }
}
