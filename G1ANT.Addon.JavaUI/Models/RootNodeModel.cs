using G1ANT.Addon.JavaUI.Services;

namespace G1ANT.Addon.JavaUI.Models
{
    public class RootNodeModel : NodeModel
    {
        public RootNodeModel()
            : base(new AccessibleRootNode(new NodeService(new AccessBridgeFactory().GetAccessBridge())))
        { }
    }
}

