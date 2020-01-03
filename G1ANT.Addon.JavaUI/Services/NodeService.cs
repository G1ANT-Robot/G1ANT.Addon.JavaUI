using G1ANT.Addon.JavaUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using WindowsAccessBridgeInterop;

namespace G1ANT.Addon.JavaUI.Services
{
    public class NodeService : INodeService
    {
        private readonly AccessBridge accessBridge;

        public NodeService(AccessBridge accessBridge)
        {
            this.accessBridge = accessBridge;
        }

        public IReadOnlyList<AccessibleJvm> GetJvms()
        {
            return accessBridge.EnumJvms(hwnd => accessBridge.CreateAccessibleWindow(hwnd));
        }

        public IReadOnlyList<NodeModel> GetJvmNodes()
        {
            return GetJvms()
                .Select(jvm => new NodeModel(jvm))
                .ToList();
        }

        public IReadOnlyList<NodeModel> GetChildNodes(NodeModel node)
        {
            return GetChildren(node.Node)
                .Select(ch => new NodeModel(ch))
                .ToList();
        }

        public IReadOnlyList<AccessibleNode> GetChildren(AccessibleNode node)
        {
            return node.GetChildren().ToList();
        }

        public AccessibleContextInfo GetNodeInfo(AccessibleNode node)
        {
            if (node is AccessibleWindow accessibleWindow)
                return accessibleWindow.GetInfo();

            if (node is AccessibleContextNode accessibleContextNode)
                return accessibleContextNode.GetInfo();

            throw new Exception($"Unsupported type of node: {node.GetType()}");
        }

        public void SetTextContents(AccessibleNode node, string text)
        {
            if (node is AccessibleContextNode accessibleNode)
            {
                if (!node.AccessBridge.Functions.SetTextContents(node.JvmId, accessibleNode.AccessibleContextHandle, text))
                    throw new Exception("SetTextContents failed");
            }
            else throw new Exception("Node is not AccessibleContextNode");
        }

        public void RequestFocus(AccessibleNode node)
        {
            if (node is AccessibleContextNode accessibleNode)
            {
                if (!node.AccessBridge.Functions.RequestFocus(node.JvmId, accessibleNode.AccessibleContextHandle))
                    throw new Exception("RequestFocus failed");
            }
            else throw new Exception("Node is not AccessibleContextNode");
        }

        public IEnumerable<string> GetActions(AccessibleContextNode node)
        {
            if (node.AccessBridge.Functions.GetAccessibleActions(node.JvmId, node.AccessibleContextHandle, out AccessibleActions accessibleActions))
            {
                if (accessibleActions.actionsCount > 0)
                {
                    return accessibleActions.actionInfo
                        .Where(a => a.name != "")
                        .Select(a => a.name)
                        .ToList();
                }
            }

            return new List<string>();
        }

        private static AccessibleActionsToDo CreateActionsToDo(string action)
        {
            var actionsToDo = new AccessibleActionsToDo()
            {
                actions = new AccessibleActionInfo[32],
                actionsCount = 1
            };
            actionsToDo.actions[0] = new AccessibleActionInfo() { name = action };
            return actionsToDo;
        }

        public void DoAction(AccessibleNode node, string action)
        {
            if (node is AccessibleContextNode accessibleNode)
            {
                var actionsToDo = CreateActionsToDo(action);

                var result = node.AccessBridge.Functions.DoAccessibleActions(
                    node.JvmId,
                    accessibleNode.AccessibleContextHandle,
                    ref actionsToDo,
                    out int failedActionIndex
                );

                if (!result)
                    throw new Exception("DoAccessibleActions failed");
            }
            else throw new Exception("Node is not AccessibleContextNode");
        }

    }
}
