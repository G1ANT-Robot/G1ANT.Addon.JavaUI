using CodePlex.XPathParser;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;

namespace G1ANT.Addon.JavaUI
{
    public class AutomationJElement
    {
    }

    public class UIElement
    {
        public static UIElement RootElement { get; set; } = null;

        protected AutomationJElement AutomationJElement;

        private UIElement()
        {
        }

        public UIElement(AutomationJElement element)
        {
            AutomationJElement = element ?? throw new NullReferenceException("Cannot create UIElement class from empty AutomationJElement");
        }

        //public static UIElement FromJPath(JPathStructure wpath)
        //{
        //    object xe = new XPathParser<object>().Parse(wpath.Value, new XPathUIElementBuilder(RootElement?.AutomationJElement));
        //    if (xe is AutomationJElement element)
        //    {
        //        return new UIElement()
        //        {
        //            AutomationJElement = element
        //        };
        //    }
        //    throw new NullReferenceException($"Cannot find UI element described by \"{wpath.Value}\".");
        //}

        //public JPathStructure ToJPath(UIElement root = null)
        //{
        //    Stack<NodeDescription> stack = new Stack<NodeDescription>();
        //    TreeWalker walker = TreeWalker.ControlViewWalker;
        //    AutomationJElement elementParent;
        //    AutomationJElement node = AutomationJElement;
        //    AutomationJElement automationRoot = root != null ? root.AutomationJElement : AutomationJElement.RootElement;
        //    do
        //    {
        //        stack.Push(new NodeDescription()
        //        {
        //            id = node.Current.AutomationId,
        //            name = node.Current.Name,
        //            className = node.Current.ClassName,
        //            type = node.Current.ControlType
        //        });
        //        elementParent = walker.GetParent(node);
        //        if (elementParent == automationRoot)
        //            break;
        //        node = elementParent;
        //    }
        //    while (true);

        //    bool parentIsEmpty = false;
        //    string wpath = "";
        //    foreach (var elem in stack)
        //    {
        //        if (string.IsNullOrEmpty(elem.id) && string.IsNullOrEmpty(elem.name))
        //            parentIsEmpty = true;
        //        else
        //        {
        //            string xpath = "";
        //            if (parentIsEmpty)
        //                xpath += "descendant::";
        //            if (string.IsNullOrEmpty(elem.id) == false)
        //                xpath += $"ui[@id='{elem.id}']";
        //            else if (string.IsNullOrEmpty(elem.name) == false)
        //                xpath += $"ui[@name='{elem.name}']";
        //            wpath += $"/{xpath}";
        //            parentIsEmpty = false;
        //        }
        //    }
        //    return new JPathStructure(wpath);
        //}

        //public void Click()
        //{
        //    if (AutomationJElement.TryGetClickablePoint(out var pt))
        //    {
        //        var tempPos = MouseWin32.GetPhysicalCursorPosition();
        //        var currentPos = new Point(tempPos.X, tempPos.Y);
        //        var targetPos = new Point((int)pt.X, (int)pt.Y);

        //        List<MouseStr.MouseEventArgs> mouseArgs =
        //            MouseStr.ToMouseEventsArgs(
        //                targetPos.X,
        //                targetPos.Y,
        //                currentPos.X,
        //                currentPos.Y,
        //                "left",
        //                "press",
        //                1);

        //        foreach (var arg in mouseArgs)
        //        {
        //            MouseWin32.MouseEvent(arg.dwFlags, arg.dx, arg.dy, arg.dwData);
        //            Thread.Sleep(10);
        //        }
        //    }
        //    else if (AutomationJElement.TryGetCurrentPattern(InvokePattern.Pattern, out var invokePattern))
        //    {
        //        (invokePattern as InvokePattern)?.Invoke();
        //    }
        //    else if (AutomationJElement.TryGetCurrentPattern(SelectionItemPattern.Pattern, out var selectionPattern))
        //    {
        //        (selectionPattern as SelectionItemPattern)?.Select();
        //    }
        //}

        //public void SetFocus()
        //{
        //    //if (AutomationJElement.Current.NativeWindowHandle != 0)
        //    //{
        //    //    IntPtr wndHandle = new IntPtr(AutomationJElement.Current.NativeWindowHandle);
        //    //    RobotWin32.SetFocus(wndHandle);
        //    //}
        //    //else
        //    AutomationJElement.SetFocus();
        //}

        //public void SetText(string text, int timeout)
        //{
        //    object valuePattern = null;
        //    if (AutomationJElement.TryGetCurrentPattern(ValuePattern.Pattern, out valuePattern))
        //    {
        //        AutomationJElement.SetFocus();
        //        ((ValuePattern)valuePattern).SetValue(text);
        //    }
        //    else if (AutomationJElement.Current.NativeWindowHandle != 0)
        //    {
        //        AutomationJElement.SetFocus();
        //        IntPtr wndHandle = new IntPtr(AutomationJElement.Current.NativeWindowHandle);
        //        KeyboardTyper.TypeWithSendInput($"{SpecialChars.KeyBegin}ctrl+home{SpecialChars.KeyEnd}", null, wndHandle, IntPtr.Zero, timeout, false, 0); // Move to start of control
        //        KeyboardTyper.TypeWithSendInput($"{SpecialChars.KeyBegin}ctrl+shift+end{SpecialChars.KeyEnd}", null, wndHandle, IntPtr.Zero, timeout, false, 0); // Select everything
        //        KeyboardTyper.TypeWithSendInput(text, null, wndHandle, IntPtr.Zero, timeout, false, 0);
        //    }
        //    else
        //        throw new NotSupportedException("SetText is not supported");
        //}

        //public System.Windows.Rect GetRectangle()
        //{
        //    object boundingRectNoDefault =
        //        AutomationJElement.GetCurrentPropertyValue(AutomationJElement.BoundingRectangleProperty, true);
        //    if (boundingRectNoDefault != AutomationJElement.NotSupported)
        //        return (System.Windows.Rect)boundingRectNoDefault;
        //    else if (AutomationJElement.Current.NativeWindowHandle != 0)
        //    {
        //        RobotWin32.Rect rect = new RobotWin32.Rect();
        //        IntPtr wndHandle = new IntPtr(AutomationJElement.Current.NativeWindowHandle);
        //        if (RobotWin32.GetWindowRectangle(wndHandle, ref rect))
        //            return new System.Windows.Rect(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
        //    }
        //    throw new NotSupportedException("Cannot get rectangle for that kind of UI element.");
        //}

        //public string GetText()
        //{
        //    //if (AutomationJElement.Current.NativeWindowHandle != 0)
        //    //{
        //    //    IntPtr wndHandle = new IntPtr(AutomationJElement.Current.NativeWindowHandle);
        //    //    return RobotWin32.GetWindowText(wndHandle);
        //    //}
        //    return AutomationJElement.Current.Name;
        //}
    }
}
