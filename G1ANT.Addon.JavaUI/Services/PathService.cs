using CodePlex.XPathParser;
using G1ANT.Addon.JavaUI.Models;
using System;
using System.Collections.Generic;

namespace G1ANT.Addon.JavaUI.Services
{
    public class PathService : IPathService
    {
        public NodeModel GetByXPath(string xpath)
        {
            var node = new XPathParser<object>().Parse(xpath, new XPathUIElementBuilder());
            if (node is NodeModel nodeModel)
                return nodeModel;

            throw new ArgumentException($"Cannot find UI element described by \"{xpath}\".");
        }

        public string GetXPathTo(NodeModel node)
        {
            var path = new List<string>();
            do
            {
                path.Add(node.ToXPath());
                node = node.GetParent();
            } while (node != null);

            path.Reverse();
            return string.Join("", path);
        }

    }
}
