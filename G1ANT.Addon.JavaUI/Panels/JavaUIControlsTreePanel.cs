using G1ANT.Addon.JavaUI.Models;
using G1ANT.Addon.JavaUI.Services;
using G1ANT.Language;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace G1ANT.Addon.JavaUI.Panels
{
    [Panel(Name = "Java windows and controls tree", DockingSide = DockingSide.Right, InitialAppear = false, Width = 400)]
    public partial class JavaUIControlsTreePanel : RobotPanel
    {
        private INodeService nodeService;
        private IPathService pathService;
        private Form markerForm;

        public JavaUIControlsTreePanel()
        {
            nodeService = new NodeService(new AccessBridgeFactory().GetAccessBridge());
            pathService = new PathService();

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
            var treeNode = new TreeNode(GetNameForNode(nodeModel))
            {
                Tag = nodeModel,
                ToolTipText = GetTooltip(nodeModel)
            };

            if (nodeModel.ChildrenCount > 0)
                treeNode.Nodes.Add("");

            return treeNode;
        }

        private string GetTooltip(NodeModel nodeModel)
        {
            var nodeProperties = nodeModel.GetType().GetProperties()
                .Where(p => p.Name != nameof(NodeModel.Node))
                .Select(p => new { Name = p.Name, Value = p.GetValue(nodeModel) })
                .Select(v => new { Name = v.Name, Value = v.Value is IEnumerable<string> ? string.Join(", ", v.Value as IEnumerable<string>) : v.Value });

            return string.Join("\r\n", nodeProperties.Where(np => !string.IsNullOrEmpty(np.Value?.ToString())).Select(np => $"{np.Name}: {np.Value}"));
        }

        private static string GetNameForNode(NodeModel nodeModel)
        {
            var name = nodeModel.Role;

            if (nodeModel.Id > 0)
                name += $" {nodeModel.Id}";

            if (!string.IsNullOrEmpty(nodeModel.Name))
            {
                name += ": ";
                name += $"\"{nodeModel.Name}\"";
            }

            return name;
        }

        private void controlsTree_AfterCollapse(object sender, TreeViewEventArgs e)
        {

        }

        private string CutControlType(string name)
        {
            return string.IsNullOrEmpty(name) ? "" : name.Replace("ControlType.", "");
        }

        private void controlsTree_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            var currentTreeNode = e.Node;
            if (currentTreeNode.Parent == null)
                return; // don't clear jvms and their windows as they are already rendered

            var node = (NodeModel)currentTreeNode.Tag;
            currentTreeNode.Nodes.Clear();

            var children = node.GetChildren();
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
                var path = pathService.GetXPathTo(nodeModel);

                if (MainForm == null) MessageBox.Show(path);
                else
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


        private void highlightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowMarkerFormForSelectedNode();
        }

        private Timer blinkTimer;
        private int blinkTimes;
        private void ShowMarkerForm(Rectangle rect)
        {
            StopBlinking();

            markerForm = new Form
            {
                ShowInTaskbar = false,
                TransparencyKey = Color.Pink,
                BackColor = Color.Red,
                ForeColor = Color.Red,
                TopMost = true,
                FormBorderStyle = FormBorderStyle.None,
                ControlBox = false,
                Text = string.Empty,
                StartPosition = FormStartPosition.Manual,
                MinimumSize = new Size(10, 10),
                Location = new Point(rect.Left, rect.Top),
                Size = new Size(rect.Right - rect.Left, rect.Bottom - rect.Top)
            };

            var transparentPanel = new Panel
            {
                BackColor = Color.Pink,
                Location = new Point(3, 3),
                Padding = new Padding(30),
                Parent = markerForm,
                Size = new Size(markerForm.Size.Width - 6, markerForm.Size.Height - 6)
            };

            markerForm.Controls.Add(transparentPanel);
            markerForm.Shown += RectangleForm_Shown;
            markerForm.Show();
        }

        private void RectangleForm_Shown(object sender, EventArgs e)
        {
            blinkTimer = new Timer() { Interval = 300, Tag = (Form)sender };
            blinkTimes = 10;
            blinkTimer.Tick -= BlinkTimer_Tick;
            blinkTimer.Tick += BlinkTimer_Tick;
            blinkTimer.Enabled = true;
        }

        private void BlinkTimer_Tick(object sender, EventArgs e)
        {
            markerForm.Visible = !markerForm.Visible;
            if (blinkTimes-- == 0)
            {
                StopBlinking();
            }
        }

        private void StopBlinking()
        {
            if (markerForm != null && blinkTimer != null)
            {
                blinkTimer.Stop();
                blinkTimer.Dispose();
                blinkTimer = null;

                markerForm.Close();
                markerForm.Dispose();
                markerForm = null;
            }
        }

        private void controlsTree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            //controlsTree.SelectedNode = e.Node;
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip.Show(MousePosition);
            }
        }

        private void controlsTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
        }

        private void ShowMarkerFormForSelectedNode()
        {
            var selectedNode = controlsTree.SelectedNode;
            if (selectedNode != null)
            {
                var bounds = ((NodeModel)selectedNode.Tag).Bounds;

                if (!bounds.IsEmpty && bounds.Width > 0 && bounds.Height >= 0)
                    ShowMarkerForm(bounds);
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            ShowMarkerFormForSelectedNode();
        }
    }
}
