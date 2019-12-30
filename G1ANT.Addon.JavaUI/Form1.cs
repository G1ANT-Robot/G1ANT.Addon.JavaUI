using G1ANT.Addon.JavaUI.PathParser;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using WindowsAccessBridgeInterop;

namespace G1ANT.Addon.JavaUI
{
    public partial class Form1 : Form
    {
        //private readonly AccessBridge _accessBridge = new AccessBridgeFactory().GetAccessBridge();
        //private readonly HwndCache _windowCache = new HwndCache();
        private PathService pathService;

        public class HwndCache
        {
            private readonly ConcurrentDictionary<IntPtr, AccessibleWindow> _cache = new ConcurrentDictionary<IntPtr, AccessibleWindow>();

            public AccessibleWindow Get(AccessBridge accessBridge, IntPtr hwnd)
            {
                return _cache.GetOrAdd(hwnd, key => accessBridge.CreateAccessibleWindow(key));
            }

            public void Clear()
            {
                _cache.Clear();
            }

            public IEnumerable<AccessibleWindow> Windows
            {
                get { return _cache.Values.Where(x => x != null); }
            }
        }


        //public List<AccessibleJvm> EnumJvms()
        //{
        //    _accessBridge.Initialize();
        //    return _accessBridge.EnumJvms(hwnd => _windowCache.Get(_accessBridge, hwnd));
        //}


        //public void Run()
        //{
        //    //AccessibleContext Ac = new AccessibleContext();
        //    //AccessibleContextInfo Info = new AccessibleContextInfo();
        //    NativeMethods.EnumWindows((hWnd, lParam) =>
        //    {
        //        if (_accessBridge.Functions.IsJavaWindow(hWnd))
        //        {
        //        }
        //        return true;
        //    }, IntPtr.Zero);
        //}

        public Form1()
        {
            InitializeComponent();
        }

        dynamic nodeService;
        private void Form1_Load(object sender, EventArgs e)
        {
            //_accessBridge.Functions.Windows_run();
            //var a = Assembly.LoadFrom(@"G1ANT.Addon.JavaUI.exe");
            //var accessBridgeFactoryType = a.ExportedTypes.Single(t => t.Name == "AccessBridgeFactory");
            //var nodeServiceType = a.ExportedTypes.Single(t => t.Name == "NodeService");

            //dynamic accessBridgeFactory = Activator.CreateInstance(accessBridgeFactoryType);
            //dynamic accessBridge = accessBridgeFactory.GetAccessBridge();
            //accessBridge.Functions.Windows_run();
            new AccessBridgeFactory().GetAccessBridge().Functions.Windows_run();
            //nodeService = Activator.CreateInstance(nodeServiceType, new AccessBridgeFactory().GetAccessBridge()); //(object)accessBridge);
            nodeService = new NodeService(new AccessBridgeFactory().GetAccessBridge());
        }


        private void button1_Click(object sender, EventArgs e)
        {
            var jvms = nodeService.GetJvms();
        }
            //pathService = new PathService(new PathParser.PathParser(), new NodeService(new AccessBridgeFactory().GetAccessBridge()));
            //var el = pathService.GetNode("/*/type=frame/type=root pane[0]/type=layered pane/type=panel/type=panel/type=panel/type=menu bar/Help");
            //el.DoAction("click");


            //textBox1.Text = "";
            //var jvms = EnumJvms();
            ////int level = 0;
            //string getIndented(string s, int level) => new string(' ', level*2) + s;

            //string traverse(AccessibleNode parent, int level = 0) {
            //    if (!(parent is AccessibleContextNode))
            //        throw new Exception();

            //    var node = (AccessibleContextNode)parent;
            //    var info = node.GetInfo();

            //    //var p = node.FetchNodeInfo();

            //    string actions = "";
            //    if (node.AccessBridge.Functions.GetAccessibleActions(node.JvmId, node.AccessibleContextHandle, out AccessibleActions accessibleActions))
            //    {
            //        if (accessibleActions.actionsCount > 0)
            //            actions += "actions: " + string.Join(",", accessibleActions.actionInfo.Take(accessibleActions.actionsCount).Select(a => a.name));
            //    }

            //    var result = getIndented(
            //        $"{parent.GetTitle()} descr:{info.description} {actions}",
            //        level
            //    );
            //    result += "\r\n";

            //    foreach (var child in parent.GetChildren())
            //        result += traverse(child, level + 1);

            //    return result;
            //};

            //foreach (var jvm in jvms)
            //{
            //    textBox1.Text += jvm.ToString();
            //    textBox1.Text += "\r\n";

            //    foreach (var window in jvm.Windows)
            //    {
            //        var info = window.FetchNodeInfo();
            //        textBox1.Text += getIndented(info.name, 0);
            //        textBox1.Text += "\r\n";

            //        foreach (var node in window.GetChildren())
            //            textBox1.Text += traverse(node);
            //    }

            //}

            //var action = new AccessibleActionsToDo()
            //{
            //    actions = new AccessibleActionInfo[32],
            //    actionsCount = 1
            //};
            //action.actions[0] = new AccessibleActionInfo() { name = "click" };

            //var _jvm = jvms[0];
            //var w = _jvm.Windows[0];
            //var n = w.FetchChildNode(0);
            ////var context = node.

            //_accessBridge.Functions.GetAccessibleActions(_jvm.JvmId, w.AccessibleContextHandle, out AccessibleActions aa);

            //_accessBridge.Functions.DoAccessibleActions(_jvm.JvmId, w.AccessibleContextHandle, ref action, out int failure);

        //}
    }
}
