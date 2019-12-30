using G1ANT.Addon.JavaUI.PathParser;
using G1ANT.Language;
using System;
using System.Linq;
using System.Windows.Forms;

namespace G1ANT.Addon.JavaUI.Panels
{
    [Panel(Name = "Java windows and controls tree", DockingSide = DockingSide.Right, InitialAppear = false, Width = 400)]
    public partial class JavaUIControlsTreePanel : RobotPanel
    {
        //private Form blinkingRectForm;

        private INodeService nodeService;
        private IPathService pathService;

        public JavaUIControlsTreePanel()
        {
            nodeService = new NodeService(new AccessBridgeFactory().GetAccessBridge());
            pathService = new PathService(new PathParser.PathParser(), nodeService);

            InitializeComponent();
        }

        public JavaUIControlsTreePanel(INodeService nodeService, IPathService pathService) // IoC
        {
            this.nodeService = nodeService;
            this.pathService = pathService;

            InitializeComponent();
        }


        public override void Initialize(IMainForm mainForm)
        {
            base.Initialize(mainForm);
            
        }

        public override void RefreshContent()
        {
            InitRootElements();
        }

        private void InitRootElements()
        {
            controlsTree.BeginUpdate();
            controlsTree.Nodes.Clear();
            var jvms = nodeService.GetJvmNodes();

            foreach (var jvm in jvms)
            {
                var name = $"{jvm.Name} {jvm.JvmId}";
                var rootNode = controlsTree.Nodes.Add(name);
                rootNode.Tag = jvm;

                var windows = nodeService.GetChildNodes(jvm);
                rootNode.Nodes.AddRange(windows.Select(w => CreateTreeNode(w)).ToArray());

                rootNode.Expand();
            }

            controlsTree.EndUpdate();
        }

        private TreeNode CreateTreeNode(NodeModel nodeModel)
        {
            var name = nodeModel.Role;

            if (nodeModel.Id > 0)
                name += $" {nodeModel.Id}";
            if (!string.IsNullOrEmpty(nodeModel.Name))
            {
                name += ": ";
                name += $"\"{nodeModel.Name}\"";
            }

            var treeNode = new TreeNode(name)
            {
                Tag = nodeModel
            };

            if (nodeModel.ChildrenCount > 0)
                treeNode.Nodes.Add("");

            return treeNode;
        }

        private void controlsTree_AfterCollapse(object sender, TreeViewEventArgs e)
        {

        }

        private string CutControlType(string name)
        {
            return string.IsNullOrEmpty(name) ? "" : name.Replace("ControlType.", "");
        }

        //private string GetTreeNodeName(AutomationElement element)
        //{
        //    if (element == null)
        //        return "";
        //    string id = "";
        //    if (string.IsNullOrWhiteSpace(element.Current.AutomationId) == false)
        //        id = $" #{element.Current.AutomationId}";
        //    return $"{CutControlType(element.Current.ControlType.ProgrammaticName)}{id} \"{element.Current.Name}\"";
        //}

        //private string GetTreeNodeTooltip(AutomationElement element, int index)
        //{
        //    if (element == null)
        //        return null;
        //    StringBuilder result = new StringBuilder();

        //    if (!string.IsNullOrWhiteSpace(element.Current.AutomationId))
        //        result.AppendLine($"id: {element.Current.AutomationId}");
        //    if (element.Current.ControlType != null)
        //    {
        //        result.AppendLine($"type: {CutControlType(element.Current.ControlType.ProgrammaticName)}");
        //        result.AppendLine($"typeid: {element.Current.ControlType.Id}");
        //    }
        //    if (!string.IsNullOrWhiteSpace(element.Current.ClassName))
        //        result.AppendLine($"class: {element.Current.ClassName}");
        //    if (!string.IsNullOrWhiteSpace(element.Current.Name))
        //        result.AppendLine($"name: {element.Current.Name}");
        //    result.AppendLine($"control index: {index}");
        //    return result.ToString();
        //}

        private void controlsTree_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            var currentTreeNode = e.Node;
            if (currentTreeNode.Parent == null)
                return; // don't clear jvms and their windows as they are already displayed

            var node = (NodeModel)currentTreeNode.Tag;
            currentTreeNode.Nodes.Clear();

            var children = nodeService.GetChildNodes(node);
            foreach (var child in children)
            {
                currentTreeNode.Nodes.Add(CreateTreeNode(child));
            }
        }

        private void InsertPathIntoScript()
        {
            if (controlsTree.SelectedNode != null)
            {
                var nodeModel = (NodeModel)controlsTree.SelectedNode.Tag;
                var path = pathService.GetPathTo(nodeModel);

                if (MainForm == null) MessageBox.Show(path); else
                MainForm.InsertTextIntoCurrentEditor($"{SpecialChars.Text}{path}{SpecialChars.Text}");
            }
        }

        private void controlsTree_DoubleClick(object sender, EventArgs e)
        {
            InsertPathIntoScript();
        }

        private void insertWPathButton_Click(object sender, EventArgs e)
        {
            InsertPathIntoScript();
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            InitRootElements();
        }


        //private AutomationElement GetTopLevelWindow(AutomationElement element)
        //{
        //    AutomationElement elementParent = TreeWalker.ControlViewWalker.GetParent(element);
        //    return elementParent == AutomationElement.RootElement ? element : GetTopLevelWindow(elementParent);
        //}

        private void highlightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (controlsTree.SelectedNode != null)
                {
                    //if (controlsTree.SelectedNode.Tag is AutomationElement automationElement)
                    //{
                    //    UIElement uiELement = new UIElement(automationElement);
                    //    var element = UIElement.FromWPath(uiELement.ToWPath());
                    //    if (element != null)
                    //    {
                    //        var window = GetTopLevelWindow(automationElement);
                    //        if (window != null)
                    //        {
                    //            var iHandle = new IntPtr(window.Current.NativeWindowHandle);
                    //            if (iHandle != IntPtr.Zero)
                    //            {
                    //                RobotWin32.BringWindowToFront(iHandle);
                    //                var rect = element.GetRectangle();
                    //                if (rect != null)
                    //                {
                    //                    InitializeRectangleForm(rect);
                    //                }
                    //            }
                    //        }
                    //    }
                    //}
                }
            }
            catch (Exception ex)
            {
                RobotMessageBox.Show(ex.Message, "Error");
            }
        }

        //private Timer blinkTimer;
        //private int blinkTimes;
        //private void InitializeRectangleForm(System.Windows.Rect rect)
        //{
        //    blinkingRectForm = new Form();
        //    Panel transparentPanel = new Panel();
        //    transparentPanel.BackColor = Color.Pink;
        //    transparentPanel.Location = new Point(3, 3);
        //    transparentPanel.Padding = new System.Windows.Forms.Padding(30);
        //    transparentPanel.Parent = blinkingRectForm;
        //    blinkingRectForm.Controls.Add(transparentPanel);
        //    blinkingRectForm.ShowInTaskbar = false;
        //    blinkingRectForm.TransparencyKey = Color.Pink;
        //    blinkingRectForm.BackColor = Color.Red;
        //    blinkingRectForm.ForeColor = Color.Red;
        //    blinkingRectForm.TopMost = true;
        //    blinkingRectForm.FormBorderStyle = FormBorderStyle.None;
        //    blinkingRectForm.ControlBox = false;
        //    blinkingRectForm.Text = string.Empty;
        //    blinkingRectForm.StartPosition = FormStartPosition.Manual;
        //    blinkingRectForm.MinimumSize = new Size(10, 10);
        //    blinkingRectForm.Location = new Point((int)rect.Left, (int)rect.Top);
        //    blinkingRectForm.Size = new Size((int)(rect.Right - rect.Left), (int)(rect.Bottom - rect.Top));
        //    transparentPanel.Size = new Size(blinkingRectForm.Size.Width - 6, blinkingRectForm.Size.Height - 6);
        //    blinkingRectForm.Shown += RectangleForm_Shown;
        //    blinkingRectForm.Show();
        //}

        //private void RectangleForm_Shown(object sender, EventArgs e)
        //{
        //    blinkTimer = new Timer();
        //    blinkTimer.Interval = 300;
        //    blinkTimes = 10;
        //    blinkTimer.Tick -= BlinkTimer_Tick;
        //    blinkTimer.Tick += BlinkTimer_Tick;
        //    blinkTimer.Enabled = true;
        //}

        //private void BlinkTimer_Tick(object sender, EventArgs e)
        //{
        //    blinkingRectForm.Visible = !blinkingRectForm.Visible;
        //    if(blinkTimes-- == 0)
        //    {
        //        blinkTimer.Enabled = false;
        //        blinkingRectForm.Close();
        //    }
        //}

        //#endregion

        private void controlsTree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            controlsTree.SelectedNode = e.Node;
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip.Show(Control.MousePosition);
            }
        }
    }
}
