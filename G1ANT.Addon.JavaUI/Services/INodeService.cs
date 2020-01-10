using G1ANT.Addon.JavaUI.Models;
using System.Collections.Generic;
using WindowsAccessBridgeInterop;

namespace G1ANT.Addon.JavaUI.Services
{
    public interface INodeService
    {
        IReadOnlyList<AccessibleJvm> GetJvms();
        IReadOnlyList<NodeModel> GetJvmNodes();

        IReadOnlyList<AccessibleNode> GetChildren(AccessibleNode node);
        IReadOnlyList<NodeModel> GetChildNodes(NodeModel node);

        AccessibleContextInfo GetNodeInfo(AccessibleNode node);

        void SetTextContents(AccessibleNode node, string text);
        string GetTextContents(AccessibleNode node);
        void RequestFocus(AccessibleNode node);

        IEnumerable<string> GetActions(AccessibleContextNode node);
        void DoAction(AccessibleNode node, string action);
    }
}
