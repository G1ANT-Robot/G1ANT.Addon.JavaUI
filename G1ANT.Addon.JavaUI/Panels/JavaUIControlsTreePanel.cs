using G1ANT.Addon.JavaUI.Controllers;
using G1ANT.Language;
using System;
using System.Windows.Forms;

namespace G1ANT.Addon.JavaUI.Panels
{
    [Panel(Name = "Java windows and controls tree", DockingSide = DockingSide.Right, InitialAppear = false, Width = 400, 
        Description = "Panel with java windows and controls tree accessible through Java Access Bridge")]
    public partial class JavaUIControlsTreePanel : RobotPanel
    {
        private JavaUIControlsTreeController controller;

        public JavaUIControlsTreePanel() : this(new JavaUIControlsTreeController())
        { }

        public JavaUIControlsTreePanel(JavaUIControlsTreeController controller) // IoC
        {
            this.controller = controller;
            InitializeComponent();
        }


        public override void Initialize(IMainForm mainForm)
        {
            base.Initialize(mainForm);
            controller.Initialize(mainForm);
            controller.SetForceIndex(forceIndexButton.Checked);
        }

        public override void RefreshContent() => controller.ReloadOnce(controlsTree);

        private void controlsTree_BeforeExpand(object sender, TreeViewCancelEventArgs e) => controller.LoadChildNodes(e.Node);

        private void controlsTree_DoubleClick(object sender, EventArgs e) => controller.InsertPathIntoScript(controlsTree.SelectedNode);

        private void insertWPathButton_Click(object sender, EventArgs e) => controller.InsertPathIntoScript(controlsTree.SelectedNode);

        private void refreshButton_Click(object sender, EventArgs e) => controller.Reload(controlsTree);

        private void highlightToolStripMenuItem_Click(object sender, EventArgs e) => controller.ShowMarkerForm(controlsTree.SelectedNode);

        private void controlsTree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                controlsTree.SelectedNode = e.Node;
                contextMenuStrip.Show(MousePosition);
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e) => controller.ShowMarkerForm(controlsTree.SelectedNode);

        private void copyNodeDetailsToolStripMenuItem_Click(object sender, EventArgs e) => controller.CopyNodeDetails(controlsTree.SelectedNode);

        private void forceIndexButton_Click(object sender, EventArgs e) => controller.SetForceIndex(forceIndexButton.Checked);  
    }
}
