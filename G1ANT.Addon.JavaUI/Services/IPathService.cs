using System.Collections.Generic;

namespace G1ANT.Addon.JavaUI.Services
{
    public interface IPathService
    {
        List<NodeModel> GetMultipleNodes(string path);
        NodeModel GetNode(string path);

        string GetPathTo(NodeModel node);
    }
}