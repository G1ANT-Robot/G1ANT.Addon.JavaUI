using G1ANT.Addon.JavaUI.Models;
using System.Collections.Generic;

namespace G1ANT.Addon.JavaUI.Services
{
    public interface IPathService
    {
        NodeModel GetByXPath(string xpath);

        string GetXPathTo(NodeModel node);
    }
}