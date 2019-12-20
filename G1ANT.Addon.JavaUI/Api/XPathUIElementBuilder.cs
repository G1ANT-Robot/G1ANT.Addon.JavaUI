//using System;
//using CodePlex.XPathParser;
//using System.Collections.Generic;
//using G1ANT.Addon.JavaUI;

//using CompareFunc = System.Func<G1ANT.Addon.JavaUI.AutomationJElement, int, bool>;
//using GetElementFunc = System.Func<G1ANT.Addon.JavaUI.AutomationJElement, G1ANT.Addon.JavaUI.AutomationJElement>;
//using FindElementFunc = System.Func<
//    G1ANT.Addon.JavaUI.AutomationJElement,
//    System.Func<G1ANT.Addon.JavaUI.AutomationJElement, int, bool>,
//    G1ANT.Addon.JavaUI.AutomationJElement>;

//namespace G1ANT.Addon.JavaUI
//{
//    public class XPathUIElementBuilder : IXPathBuilder<object>
//    {
//        public enum UiAutomationJElement
//        {
//            Id,
//            ProgrammaticName,
//        }

//        protected AutomationJElement FindDescendant(AutomationJElement elem, CompareFunc compare)
//        {
//            AutomationJElement elementNode = TreeWalker.ControlViewWalker.GetFirstChild(elem);
//            int index = 0;
//            while (elementNode != null)
//            {
//                if (compare(elementNode, index))
//                    return elementNode;
//                var descendantElement = FindDescendant(elementNode, compare);
//                if (descendantElement != null)
//                    return descendantElement;
//                elementNode = TreeWalker.ControlViewWalker.GetNextSibling(elementNode);
//                index++;
//            }
//            return null;
//        }

//        protected AutomationJElement FindChild(AutomationJElement elem, CompareFunc compare)
//        {
//            AutomationJElement elementNode = TreeWalker.ControlViewWalker.GetFirstChild(elem);
//            int index = 0;
//            while (elementNode != null)
//            {
//                if (compare(elementNode, index))
//                    return elementNode;
//                elementNode = TreeWalker.ControlViewWalker.GetNextSibling(elementNode);
//                index++;
//            }
//            //return null;
//            //var childrens = elem.FindAll(TreeScope.Children, PropertyCondition.TrueCondition);
//            //foreach (AutomationJElement child in childrens)
//            //{
//            //    if (compare(child))
//            //        return child;
//            //}
//            throw new ElementNotAvailableException();
//        }

//        protected AutomationJElement FindFollowingSibling(AutomationJElement elem, CompareFunc compare)
//        {
//            AutomationJElement elementNode = TreeWalker.ControlViewWalker.GetFirstChild(elem);
//            int index = 0;
//            while (elementNode != null)
//            {
//                if (compare(elementNode, index))
//                    return elementNode;
//                elementNode = TreeWalker.ControlViewWalker.GetNextSibling(elementNode);
//                index++;
//            }
//            throw new ElementNotAvailableException();
//        }

//        protected AutomationJElement FindDescendantOrSelf(AutomationJElement elem, CompareFunc compare)
//        {
//            if (compare(elem, -1))
//                return elem;
//            return FindDescendant(elem, compare);
//        }

//        public AutomationJElement Root { get; } = AutomationJElement.RootElement;
//        public XPathUIElementBuilder(AutomationJElement root = null)
//        {
//            ControlType.Button.GetType();
//            if (root != null)
//                Root = root;
//        }

//        public void StartBuild()
//        {
//        }

//        public object EndBuild(object result)
//        {
//            return result;
//        }

//        public object String(string value)
//        {
//            return value;
//        }

//        public object Number(string value)
//        {
//            int result = -1;
//            if (int.TryParse(value, out result))
//                return result;
//            throw new NotSupportedException($"Number '{value}' is not supported.");
//        }

//        public object Operator(XPathOperator op, object left, object right)
//        {
//            if (op == XPathOperator.Eq)
//            {
//                if (left is AutomationProperty property)
//                {
//                    return new CompareFunc((elem, index) =>
//                     {
//                         var propValue = elem.GetCurrentPropertyValue(property, true);
//                         if (propValue != null)
//                             return propValue.Equals(right);
//                         return false;
//                     });
//                }
//                else if (left is UiAutomationJElement en)
//                {
//                    if (UiAutomationJElement.ProgrammaticName == en)
//                    {
//                        return new CompareFunc((elem, index) =>
//                        {
//                            string propValue = elem.Current.ControlType?.ProgrammaticName.Replace("ControlType.", "");
//                            if (propValue != null)
//                                return propValue.Equals(right);
//                            return false;
//                        });
//                    }
//                    if (UiAutomationJElement.Id == en)
//                    {
//                        return new CompareFunc((elem, index) =>
//                        {
//                            int? propValue = elem.Current.ControlType?.Id;
//                            if (propValue.HasValue)
//                                return propValue.ToString().Equals(right);
//                            return false;
//                        });
//                    }
//                }
//            }
//            throw new NotSupportedException($"Operator {op.ToString()} is not supported.");
//        }

//        public object Axis(XPathAxis xpathAxis, System.Xml.XPath.XPathNodeType nodeType, string prefix, string name)
//        {
//            if (xpathAxis == XPathAxis.Root)
//                return Root;
//            if (nodeType == System.Xml.XPath.XPathNodeType.Element)
//            {
//                if (name != "ui")
//                    throw new NotSupportedException($"{name} element is not supported.");
//            }
//            if (xpathAxis == XPathAxis.Descendant)
//            {
//                FindElementFunc func = FindDescendant;
//                return func;
//            }
//            if (xpathAxis == XPathAxis.DescendantOrSelf)
//            {
//                FindElementFunc func = FindDescendantOrSelf;
//                return func;
//            }
//            if (xpathAxis == XPathAxis.FollowingSibling)
//            {
//                FindElementFunc func = FindFollowingSibling;
//                return func;
//            }
//            if (xpathAxis == XPathAxis.Child)
//            {
//                FindElementFunc func = FindChild;
//                return func;
//            }
//            if (xpathAxis == XPathAxis.Attribute)
//            {
//                string lowerCaseName = name.ToLower();
//                if (lowerCaseName == "id")
//                    return AutomationJElement.AutomationIdProperty;
//                if (lowerCaseName == "name")
//                    return AutomationJElement.NameProperty;
//                if (lowerCaseName == "class")
//                    return AutomationJElement.ClassNameProperty;
//                if (lowerCaseName == "type")
//                    return UiAutomationJElement.ProgrammaticName;
//                if (lowerCaseName == "typeid")
//                    return UiAutomationJElement.Id;
//                throw new NotSupportedException($"Attribute {name} is not supportet.");
//            }
//            return null;
//        }

//        public object JoinStep(object left, object right)
//        {
//            if (left is AutomationJElement elem &&
//                right is GetElementFunc func)
//            {
//                return func(elem);
//            }
//            if (left is GetElementFunc inner &&
//                right is GetElementFunc outer)
//            {
//                GetElementFunc retFunc = (element) =>
//                {
//                    var ret = inner(element);
//                    if (ret == null)
//                        throw new ElementNotAvailableException();
//                    return outer(ret);
//                };
//                return retFunc;
//            }
//            return null;
//        }

//        public object Predicate(object node, object condition, bool reverseStep)
//        {
//            if (node is FindElementFunc outer1 &&
//                condition is CompareFunc inner1)
//            {
//                GetElementFunc func = (elem) =>
//                {
//                    return outer1(elem, inner1);
//                };
//                return func;
//            }
//            else if (node is FindElementFunc outer2 &&
//                condition is int value)
//            {
//                GetElementFunc func = (elem) =>
//                {
//                    return outer2(elem, (childElem, childIndex) => { return childIndex == value; });
//                };
//                return func;
//            }
//            return null;
//        }

//        public object Variable(string prefix, string name)
//        {
//            throw new NotImplementedException("Method 'Variable' is not implemented.");
//        }

//        public object Function(string prefix, string name, IList<object> args)
//        {
//            if (name.ToLower() == "contains" && args.Count == 2)
//            {
//                if (args[0] is AutomationProperty property &&
//                    args[1] is string text)
//                {
//                    CompareFunc func = (elem, index) =>
//                    {
//                        if (elem.GetCurrentPropertyValue(property, true) is string str)
//                        {
//                            if (str.Contains(text))
//                                return true;
//                        }
//                        return false;
//                    };
//                    return func;
//                }
//            }
//            throw new NotSupportedException($"Function {name} is not supportet.");
//        }
//    }
//}
