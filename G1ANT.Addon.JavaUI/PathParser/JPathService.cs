using System;
using System.Collections.Generic;
using System.Linq;
using WindowsAccessBridgeInterop;

namespace G1ANT.Addon.JavaUI.PathParser
{
    public class JPathService
    {
        private readonly IPathParser pathParser;
        private readonly AccessBridge accessBridge;


        public JPathService(IPathParser pathParser, AccessBridge accessBridge)
        {
            this.pathParser = pathParser;
            this.accessBridge = accessBridge;
        }


        private List<AccessibleJvm> GetMatchingJvms(PathElement pathElement)
        {
            var jvms = accessBridge.EnumJvms(hwnd => accessBridge.CreateAccessibleWindow(hwnd));
            if (pathElement.Id != 0)
                return jvms.Where(vm => vm.JvmId == pathElement.Id).ToList();
            else
                return jvms;
        }

        private List<AccessibleNode> GetMatchingChildNodes(List<AccessibleNode> parents, PathElement pathElement)
        {
            var allChildren = parents.SelectMany(parent => parent.GetChildren());
            var matchingChildren = allChildren
                .Where(child => IsMatch(child, pathElement))
                .ToList();

            if (pathElement.Index.HasValue)
                return new List<AccessibleNode>() { matchingChildren[pathElement.Index.Value] };
            else
                return matchingChildren;
        }

        private AccessibleContextInfo GetInfo(AccessibleNode node)
        {
            if (node is AccessibleWindow accessibleWindow)
                return accessibleWindow.GetInfo();

            if (node is AccessibleContextNode accessibleContextNode)
                return accessibleContextNode.GetInfo();

            throw new Exception($"Unsupported type of node: {node.GetType()}");
        }

        private bool IsMatch(AccessibleNode node, PathElement pathElement)
        {
            if (pathElement.IsWildcard)
                return true;

            if (pathElement.Id != 0)
            {
                if (node is AccessibleWindow accessibleWindow)
                    return pathElement.Id == (int)accessibleWindow.Hwnd;

                throw new Exception("Can't apply filter by id to objects other than AccessibleWindow");
            }

            var nodeInfo = GetInfo(node);
            if (pathElement.Name != null)
                return pathElement.Name == nodeInfo.name;
            if (pathElement.Role != null)
                return pathElement.Role == nodeInfo.role;
            if (pathElement.Description != null)
                return pathElement.Description == nodeInfo.description;

            return false;
        }


        private List<AccessibleNode> GetAccessibleNodes(string path)
        {
            var pathElements = pathParser.Parse(path);

            var jvms = GetMatchingJvms(pathElements[0]);
            if (!jvms.Any())
                throw new Exception($"No matching JVM found");

            pathElements = pathElements.Skip(1).ToList();
            var currentParents = jvms.Cast<AccessibleNode>().ToList();

            foreach (var pathElement in pathElements)
            {
                currentParents = GetMatchingChildNodes(currentParents, pathElement);
                if (!currentParents.Any())
                    throw new Exception($"No node found for path element {pathElement}");
            }

            return currentParents;
        }

        public List<NodeModel> GetMultiple(string path)
        {
            return GetAccessibleNodes(path)
                .Select(el => new NodeModel(el))
                .ToList();
        }

        public NodeModel Get(string path)
        {
            var nodes = GetAccessibleNodes(path);

            if (nodes.Count() > 1)
                throw new Exception($"Multiple elements found: {string.Join(", ", nodes.OfType<AccessibleContextNode>().Select(e => e.GetTitle()))}");

            return new NodeModel(nodes.Single());
        }
    }
}
