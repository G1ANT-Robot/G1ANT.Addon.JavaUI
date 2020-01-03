using G1ANT.Addon.JavaUI.Services;
using System.Collections.Generic;
using System.Linq;
using WindowsAccessBridgeInterop;

namespace G1ANT.Addon.JavaUI.Models
{
    public class AccessibleRootNode : AccessibleNode
    {
        private readonly INodeService nodeService;
        private readonly IList<AccessibleJvm> children;

        public AccessibleRootNode(INodeService nodeService)
            : base(null)
        {
            this.nodeService = nodeService;
            children = nodeService.GetJvms().ToList();
        }

        override public int JvmId => 0;
        override public AccessibleNode GetParent() => null;
        override protected int GetChildrenCount() => children.Count;
        override protected AccessibleNode GetChildAt(int i) => children[i];
        override public string GetTitle() => null;
    }
}

