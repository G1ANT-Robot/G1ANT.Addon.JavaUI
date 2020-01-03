using CodePlex.XPathParser;
using G1ANT.Addon.JavaUI.Models;
using G1ANT.Addon.JavaUI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.XPath;
using WindowsAccessBridgeInterop;
using CompareFunc = System.Func<G1ANT.Addon.JavaUI.Models.NodeModel, int, bool>;
using FindElementFunc = System.Func<
    G1ANT.Addon.JavaUI.Models.NodeModel,
    System.Func<G1ANT.Addon.JavaUI.Models.NodeModel, int, bool>,
    G1ANT.Addon.JavaUI.Models.NodeModel>;
using GetElementFunc = System.Func<G1ANT.Addon.JavaUI.Models.NodeModel, G1ANT.Addon.JavaUI.Models.NodeModel>;

namespace G1ANT.Addon.JavaUI
{

    public enum NodeProperty
    {
        JvmId,
        Id,
        Name,
        Role
    }

    public class XPathUIElementBuilder : IXPathBuilder<object>
    {
        private NodeService nodeService;

        public XPathUIElementBuilder()
        {
            nodeService = new NodeService(new AccessBridgeFactory().GetAccessBridge());
        }


        protected NodeModel FindDescendant(NodeModel node, CompareFunc compare)
        {
            var children = nodeService.GetChildNodes(node);

            int index = 0;
            foreach (var child in children)
            {
                if (compare(child, index))
                    return child;
                var descendantElement = FindDescendant(child, compare);
                if (descendantElement != null)
                    return descendantElement;

                index++;
            }
            return null;
        }

        protected NodeModel FindChild(NodeModel node, CompareFunc compare)
        {
            var children = nodeService.GetChildNodes(node);

            int index = 0;
            foreach (var child in children)
            {
                if (compare(child, index))
                    return child;
                index++;
            }
            throw new Exception("Child not found");
        }

        protected NodeModel FindFollowingSibling(NodeModel node, CompareFunc compare)
        {
            var children = nodeService.GetChildNodes(node);

            int index = 0;
            foreach (var child in children)
            {
                if (compare(child, index))
                {
                    if (children.Count < index + 1)
                        throw new Exception("No element after selected");
                    return children.ToList()[index + 1];
                }

                index++;
            }
            throw new Exception("Child not found");
        }

        protected NodeModel FindDescendantOrSelf(NodeModel node, CompareFunc compare)
        {
            if (compare(node, -1))
                return node;
            return FindDescendant(node, compare);
        }

        //public NodeModel Root { get; } = NodeModel.RootElement;
        //public XPathUIElementBuilder(NodeModel root = null)
        //{
        //ControlType.Button.GetType();
        //if (root != null)
        //    Root = root;
        //}

        public void StartBuild()
        {
        }

        public object EndBuild(object result)
        {
            return result;
        }

        public object String(string value)
        {
            return value;
        }

        public object Number(string value)
        {
            int result = -1;
            if (int.TryParse(value, out result))
                return result;
            throw new NotSupportedException($"Number '{value}' is not supported.");
        }


        private string GetPropertyValue(NodeModel node, NodeProperty property)
        {
            switch (property)
            {
                case NodeProperty.Id: return node.Id.ToString();
                case NodeProperty.JvmId: return node.JvmId.ToString();
                case NodeProperty.Name: return node.Name;
                case NodeProperty.Role: return node.Role;
                default:
                    throw new ArgumentOutOfRangeException(nameof(property), property.ToString());
            }
        }

        public object Operator(XPathOperator op, object left, object right)
        {
            if (op == XPathOperator.Eq)
            {
                if (left is NodeProperty property)
                {
                    return new CompareFunc((node, index) =>
                     {
                         var propValue = GetPropertyValue(node, property);
                         return propValue?.Equals(right) == true;
                     });
                }
                else if (left is NodeModel en)
                {
                    //if (UiNodeModel.ProgrammaticName == en)
                    //{
                    //    return new CompareFunc((elem, index) =>
                    //    {
                    //        string propValue = elem.Current.ControlType?.ProgrammaticName.Replace("ControlType.", "");
                    //        if (propValue != null)
                    //            return propValue.Equals(right);
                    //        return false;
                    //    });
                    //}
                    //if (UiNodeModel.Id == en)
                    //{
                    //    return new CompareFunc((elem, index) =>
                    //    {
                    //        int? propValue = elem.Current.ControlType?.Id;
                    //        if (propValue.HasValue)
                    //            return propValue.ToString().Equals(right);
                    //        return false;
                    //    });
                    //}
                }
            }
            throw new NotSupportedException($"Operator {op.ToString()} is not supported.");
        }

        public object Axis(XPathAxis xpathAxis, XPathNodeType nodeType, string prefix, string name)
        {
            if (xpathAxis == XPathAxis.Root)
                return new RootNodeModel(); // nodeService.GetJvmNodes(); // ????

            if (nodeType == XPathNodeType.Element && name != "ui")
                throw new NotSupportedException($"{name} element is not supported.");

            switch (xpathAxis)
            {
                case XPathAxis.Descendant: return (FindElementFunc)FindDescendant;
                case XPathAxis.DescendantOrSelf: return (FindElementFunc)FindDescendantOrSelf;
                case XPathAxis.FollowingSibling: return (FindElementFunc)FindFollowingSibling;
                case XPathAxis.Child: return (FindElementFunc)FindChild;
                case XPathAxis.Attribute:
                    {
                        switch (name.ToLower())
                        {
                            case "id":
                                return NodeProperty.Id;
                            case "jvmid":
                                return NodeProperty.JvmId;
                            case "name":
                                return NodeProperty.Name;
                            case "role":
                                return NodeProperty.Role;
                            default:
                                throw new NotSupportedException($"Attribute {name} is not supported.");
                        }
                    }
            }
            return null;
        }

        public object JoinStep(object left, object right)
        {
            if (left is NodeModel elem && right is GetElementFunc func)
            {
                return func(elem);
            }
            if (left is GetElementFunc inner && right is GetElementFunc outer)
            {
                GetElementFunc retFunc = (element) =>
                {
                    var ret = inner(element);
                    if (ret == null)
                        throw new Exception("Node not found");
                    return outer(ret);
                };
                return retFunc;
            }
            return null;
        }

        public object Predicate(object node, object condition, bool reverseStep)
        {
            if (node is FindElementFunc outer1 && condition is CompareFunc inner1)
            {
                GetElementFunc func = elem => outer1(elem, inner1);
                return func;
            }
            else if (node is FindElementFunc outer2 && condition is int value)
            {
                GetElementFunc func = elem => outer2(elem, (childElem, childIndex) => childIndex == value);
                return func;
            }

            return null;
        }

        public object Variable(string prefix, string name)
        {
            throw new NotImplementedException("Method 'Variable' is not implemented.");
        }

        public object Function(string prefix, string name, IList<object> args)
        {
            if (name.ToLower() == "contains" && args.Count == 2)
            {
                if (args[0] is NodeProperty property && args[1] is string text)
                {
                    CompareFunc func = (elem, index) => GetPropertyValue(elem, property) is string str && str.Contains(text);
                    return func;
                }
            }
            else if (name.ToLower() == "position" && args.Count == 1)
            {
                if (args[0] is int childIndex)
                {
                    CompareFunc func = (elem, index) => elem.IndexInParent == childIndex;
                    return func;
                }
            }

            throw new NotSupportedException($"Function {name} is not supported.");
        }
    }
}
