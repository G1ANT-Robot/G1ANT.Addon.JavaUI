using G1ANT.Addon.JavaUI.PathParser;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using WindowsAccessBridgeInterop;
using WindowsAccessBridgeInterop.Win32;

namespace G1ANT.Addon.JavaUI
{
    public partial class Form1 : Form
    {
        private readonly AccessBridge _accessBridge = new AccessBridge();
        private readonly HwndCache _windowCache = new HwndCache();

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


        public List<AccessibleJvm> EnumJvms()
        {
            _accessBridge.Initialize();
            return _accessBridge.EnumJvms(hwnd => _windowCache.Get(_accessBridge, hwnd));
        }


        public void Run()
        {
            //AccessibleContext Ac = new AccessibleContext();
            //AccessibleContextInfo Info = new AccessibleContextInfo();
            NativeMethods.EnumWindows((hWnd, lParam) =>
            {
                if (_accessBridge.Functions.IsJavaWindow(hWnd))
                {
                }
                return true;
            }, IntPtr.Zero);
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _accessBridge.Functions.Windows_run();
        }


        //public class TreeWalker
        //{
        //    public AccessibleContextNode GetByPath()
        //    {}
        //}

        private void button1_Click(object sender, EventArgs e)
        {
            var parser = new PathParser.PathParser();
            //var path = parser.Parse("/12061974/type=frame");


            var ps = new JPathService(parser, _accessBridge);
            var el = ps.Get("/*/type=frame/type=root pane[0]/type=layered pane/type=panel/type=panel/type=panel/type=menu bar/Help");
            el.DoAction("click");


            textBox1.Text = "";
            var jvms = EnumJvms();
            //int level = 0;
            string getIndented(string s, int level) => new string(' ', level*2) + s;

            string traverse(AccessibleNode parent, int level = 0) {
                if (!(parent is AccessibleContextNode))
                    throw new Exception();

                var node = (AccessibleContextNode)parent;
                var info = node.GetInfo();

                //var p = node.FetchNodeInfo();

                string actions = "";
                if (node.AccessBridge.Functions.GetAccessibleActions(node.JvmId, node.AccessibleContextHandle, out AccessibleActions accessibleActions))
                {
                    if (accessibleActions.actionsCount > 0)
                        actions += "actions: " + string.Join(",", accessibleActions.actionInfo.Take(accessibleActions.actionsCount).Select(a => a.name));
                }

                var result = getIndented(
                    $"{parent.GetTitle()} descr:{info.description} {actions}",
                    level
                );
                result += "\r\n";

                foreach (var child in parent.GetChildren())
                    result += traverse(child, level + 1);

                return result;
            };

            foreach (var jvm in jvms)
            {
                textBox1.Text += jvm.ToString();
                textBox1.Text += "\r\n";

                foreach (var window in jvm.Windows)
                {
                    var info = window.FetchNodeInfo();
                    textBox1.Text += getIndented(info.name, 0);
                    textBox1.Text += "\r\n";

                    foreach (var node in window.GetChildren())
                        textBox1.Text += traverse(node);
                }

            }

            var action = new AccessibleActionsToDo()
            {
                actions = new AccessibleActionInfo[32],
                actionsCount = 1
            };
            action.actions[0] = new AccessibleActionInfo() { name = "click" };

            var _jvm = jvms[0];
            var w = _jvm.Windows[0];
            var n = w.FetchChildNode(0);
            //var context = node.

            _accessBridge.Functions.GetAccessibleActions(_jvm.JvmId, w.AccessibleContextHandle, out AccessibleActions aa);
            
            _accessBridge.Functions.DoAccessibleActions(_jvm.JvmId, w.AccessibleContextHandle, ref action, out int failure);

        }
    }
}
