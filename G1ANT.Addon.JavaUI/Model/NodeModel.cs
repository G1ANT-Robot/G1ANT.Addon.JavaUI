﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using WindowsAccessBridgeInterop;

namespace G1ANT.Addon.JavaUI
{
    public class NodeModel : IDisposable
    {
        public int JvmId { get; private set; }
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        /// <summary>type</summary>
        public string Role { get; private set; }
        public List<string> Actions { get; private set; }
        public List<string> States { get; private set; }
        public Rectangle Bounds { get; private set; }
        public int ChildrenCount { get; private set; }
        public int Height { get; private set; }
        public int IndexInParent { get; private set; }
        public int Width { get; private set; }
        public int X { get; private set; }
        public int Y { get; private set; }

        private AccessibleNode node;

        public NodeModel(AccessibleNode node)
        {
            this.node = node;

            switch (node)
            {
                case AccessibleJvm accessibleJvm:
                    Id = (int)accessibleJvm.JvmId;
                    Name = accessibleJvm.GetTitle();
                    break;
                case AccessibleWindow accessibleWindow:
                    Id = (int)accessibleWindow.Hwnd;
                    FillFromAccessibleContextInfo(GetInfo(node));
                    break;
                case AccessibleContextNode accessibleContextNode:
                    FillFromAccessibleContextInfo(GetInfo(node));
                    FillActions(accessibleContextNode);
                    break;
            }
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
        }

        private void FillActions(AccessibleContextNode node)
        {
            if (node.AccessBridge.Functions.GetAccessibleActions(node.JvmId, node.AccessibleContextHandle, out AccessibleActions accessibleActions))
            {
                if (accessibleActions.actionsCount > 0)
                {
                    Actions = accessibleActions.actionInfo
                        .Where(a => a.name != "")
                        .Select(a => a.name)
                        .ToList();
                }
            }
        }


        public void DoAction(string action)
        {
            if (!Actions.Contains(action))
                throw new ArgumentOutOfRangeException($"Action {action} not found, available actions: {string.Join(", ", Actions)}");

            var actionsToDo = CreateActionsToDo(action);

            node.AccessBridge.Functions.DoAccessibleActions(
                node.JvmId,
                ((AccessibleContextNode)node).AccessibleContextHandle,
                ref actionsToDo,
                out int failure
            );

            if (failure >= 0)
                throw new Exception($"DoAccessibleActions failed with error code {failure}");
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

        public void Dispose()
        {
            node.Dispose();
        }
    }
}

