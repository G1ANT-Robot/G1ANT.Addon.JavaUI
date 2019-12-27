using System;
using System.Collections.Generic;
using System.Linq;
using WindowsAccessBridgeInterop;

namespace G1ANT.Addon.JavaUI.PathParser
{
    public class JPathService
    {
        private readonly IPathParser pathParser;
        private readonly NodeService nodeService;

        public JPathService(IPathParser pathParser, NodeService nodeService)
        {
            this.pathParser = pathParser;
            this.nodeService = nodeService;
        }


        private IReadOnlyCollection<AccessibleJvm> GetMatchingJvms(PathElement pathElement)
        {
            var jvms = nodeService.GetJvms();
            if (pathElement.Id != 0)
                return jvms.Where(vm => vm.JvmId == pathElement.Id).ToList();
            else
                return jvms;
        }

        private IReadOnlyCollection<AccessibleNode> GetMatchingChildNodes(IEnumerable<AccessibleNode> parents, PathElement pathElement)
        {
            var allChildren = parents.SelectMany(parent => nodeService.GetChildren(parent));
            var matchingChildren = allChildren
                .Where(child => IsMatch(child, pathElement))
                .ToList();

            if (pathElement.Index.HasValue)
                return new List<AccessibleNode>() { matchingChildren[pathElement.Index.Value] };
            else
                return matchingChildren;
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

            var nodeInfo = nodeService.GetNodeInfo(node);

            if (pathElement.Name != null)
                return pathElement.Name == nodeInfo.name;
            if (pathElement.Role != null)
                return pathElement.Role == nodeInfo.role;
            if (pathElement.Description != null)
                return pathElement.Description == nodeInfo.description;

            return false;
        }


        private IReadOnlyCollection<AccessibleNode> GetAccessibleNodes(string path)
        {
            var pathElements = pathParser.Parse(path);

            var jvms = GetMatchingJvms(pathElements[0]);
            if (!jvms.Any())
                throw new Exception("No matching JVM found");

            pathElements = pathElements.Skip(1).ToList();
            IReadOnlyCollection<AccessibleNode> currentParents = jvms.Cast<AccessibleNode>().ToList();

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

            if (nodes.Count() != 1)
                throw new Exception($"Multiple or no elements found: {string.Join(", ", nodes.OfType<AccessibleContextNode>().Select(e => e.GetTitle()))}");

            return new NodeModel(nodes.Single());
        }
    }
}
