using G1ANT.Addon.JavaUI.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using WindowsAccessBridgeInterop;

namespace G1ANT.Addon.JavaUI.Models
{
    public class NodeModel : IDisposable
    {
        public int JvmId { get; private set; }
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Role { get; private set; }
        public Rectangle Bounds { get; private set; }
        public int ChildrenCount { get; private set; }
        public int Height { get; private set; }
        public int IndexInParent { get; private set; }
        public int Width { get; private set; }
        public int X { get; private set; }
        public int Y { get; private set; }

        private int actionsCount;
        private Lazy<IReadOnlyCollection<string>> actions;
        public IReadOnlyCollection<string> Actions => actions.Value;

        public IReadOnlyCollection<string> States { get; private set; }


        private INodeService nodeService;

        [JsonIgnore]
        public AccessibleNode Node { get; private set; }

        public NodeModel(AccessibleNode node, INodeService nodeService = null)
        {
            Node = node ?? throw new ArgumentNullException(nameof(node));
            actions = new Lazy<IReadOnlyCollection<string>>(() => GetActions(Node));

            this.nodeService = nodeService ?? new NodeService(node.AccessBridge);

            JvmId = node.JvmId;
            switch (node)
            {
                case AccessibleJvm accessibleJvm:
                    Id = accessibleJvm.JvmId;
                    Name = accessibleJvm.GetTitle();
                    break;
                case AccessibleWindow accessibleWindow:
                    Id = (int)accessibleWindow.Hwnd;
                    FillFromAccessibleContextInfo(GetInfo(node));
                    break;
                case AccessibleContextNode accessibleContextNode:
                    FillFromAccessibleContextInfo(GetInfo(node));
                    break;
            }
        }

        public void SetTextContents(string text)
        {
            nodeService.SetTextContents(Node, text);
        }

        public string GetTextContents()
        {
            return nodeService.GetTextContents(Node);
        }

        public void RequestFocus()
        {
            nodeService.RequestFocus(Node);
        }

        public NodeModel GetParent()
        {
            var parent = Node.GetParent();
            return parent != null ? new NodeModel(parent) : null;
        }

        public NodeModel GetParentWindow()
        {
            var parent = Node;
            while (!(parent is AccessibleWindow) && parent != null)
            {
                parent = parent.GetParent();
            }

            return parent != null ? new NodeModel(parent) : null;
        }

        public void BringToFront()
        {
            nodeService.BringToFront(this);
        }

        public IReadOnlyCollection<NodeModel> GetChildren()
        {
            return nodeService.GetChildNodes(this);
        }

        private AccessibleContextInfo GetInfo(AccessibleNode node)
        {
            if (node is AccessibleWindow accessibleWindow)
                return accessibleWindow.GetInfo();

            if (node is AccessibleContextNode accessibleContextNode)
                return accessibleContextNode.GetInfo();

            throw new Exception($"Unsupported type of node: {node.GetType()}");
        }

        private void FillFromAccessibleContextInfo(AccessibleContextInfo info)
        {
            Name = info.name;
            Role = info.role;
            Description = info.description;
            States = info.states?.Split(',').ToList();
            ChildrenCount = info.childrenCount;
            Height = info.height;
            Width = info.width;
            X = info.x;
            Y = info.y;
            IndexInParent = info.indexInParent;
            Bounds = new Rectangle(X, Y, Width, Height);
            actionsCount = info.accessibleAction;
        }

        private List<string> GetActions(AccessibleNode node)
        {
            if (actionsCount > 0 && Node is AccessibleContextNode accessibleContextNode)
                return nodeService.GetActions(accessibleContextNode).ToList();

            return new List<string>();
        }

        public void DoAction(string action)
        {
            if (!Actions.Contains(action))
                throw new ArgumentOutOfRangeException($"Action {action} not found, available actions: {string.Join(", ", Actions)}");

            nodeService.DoAction(Node, action);
        }

        private string GetSpecificElementSelector()
        {
            if (!string.IsNullOrEmpty(Name))
                return $"@name='{Name}'";
            if (!string.IsNullOrEmpty(Role))
                return $"@role='{Role}'";
            if (Id != 0)
                return $"@id='{Id}'";

            return $"position()={IndexInParent}";
        }

        private string GetElementPrefix() => Node is AccessibleWindow ? "descendant::" : "";

        public string ToXPath()
        {
            if (Node is AccessibleJvm)
                return "";

            return $"/{GetElementPrefix()}ui[{GetSpecificElementSelector()}]";
        }


        public void Dispose()
        {
            Node.Dispose();
        }
    }
}

