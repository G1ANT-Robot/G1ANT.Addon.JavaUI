using G1ANT.Addon.JavaUI.Services;
using WindowsAccessBridgeInterop;

namespace G1ANT.Addon.JavaUI.Models
{
    /// <summary>
    /// AccessibleNode implementation required to ease XPath implementation (assumpts there's single root)
    /// </summary>
    public class AccessibleRootNode : AccessibleNode
    {
        private readonly INodeService nodeService;

        public AccessibleRootNode(INodeService nodeService)
            : base(null)
        {
            this.nodeService = nodeService;
        }

        override public int JvmId => 0;
        override public AccessibleNode GetParent() => null;
        override protected int GetChildrenCount() => nodeService.GetJvms().Count;
        override protected AccessibleNode GetChildAt(int i) => nodeService.GetJvms()[i];
        override public string GetTitle() => "Parent of JVMS";
    }
}