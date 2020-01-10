using G1ANT.Addon.JavaUI.Models;
using G1ANT.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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

        public string GetTextContents(AccessibleNode node)
        {
            if (node is AccessibleContextNode accessibleNode)
            {
                if (node.AccessBridge.Functions.GetAccessibleTextItems(
                    node.JvmId,
                    accessibleNode.AccessibleContextHandle,
                    out AccessibleTextItemsInfo textItemsInfo,
                    0))
                {
                    return textItemsInfo.sentence.TrimEnd('\n');
                }

                throw new Exception("GetAccessibleTextItems failed");
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
                        .Take(accessibleActions.actionsCount)
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

        public void BringToFront(NodeModel node)
        {
            var windowNode = node.GetParentWindow();
            var accessibleWindow = (AccessibleWindow)windowNode.Node;

            RobotWin32.BringWindowToFront(accessibleWindow.Hwnd);

            if (IsWindowMinimized(windowNode))
                Thread.Sleep(1000);
        }

        private static bool IsWindowMinimized(NodeModel windowNode)
        {
            return windowNode.X + windowNode.Width < 0 && windowNode.Y + windowNode.Height < 0;
        }

    }
}
