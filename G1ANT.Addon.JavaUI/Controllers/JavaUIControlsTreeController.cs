using G1ANT.Addon.JavaUI.Forms;
using G1ANT.Addon.JavaUI.Models;
using G1ANT.Addon.JavaUI.Services;
using G1ANT.Language;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace G1ANT.Addon.JavaUI.Controllers
{
    public class JavaUIControlsTreeController
    {
        private readonly INodeService nodeService;
        private readonly IPathService pathService;
        private IMainForm mainForm;
        private MarkerForm markerForm;

        public JavaUIControlsTreeController()
            : this(new NodeService(new AccessBridgeFactory().GetAccessBridge()), new PathService())
        { }

        public JavaUIControlsTreeController(INodeService nodeService, IPathService pathService) // IoC
        {
            this.nodeService = nodeService;
            this.pathService = pathService;
        }

        public void Initialize(IMainForm mainForm) => this.mainForm  = mainForm;

        public void InitRootElements(TreeView controlsTree)
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

        public void LoadChildNodes(TreeNode currentTreeNode)
        {
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

        public void InsertPathIntoScript(TreeNode node)
        {
            if (node != null)
            {
                var nodeModel = (NodeModel)node.Tag;
                var path = pathService.GetXPathTo(nodeModel);

                if (mainForm == null)
                    MessageBox.Show(path);
                else
                    mainForm.InsertTextIntoCurrentEditor($"{SpecialChars.Text}{path}{SpecialChars.Text}");
            }
        }

        public void ShowMarkerForm(TreeNode node)
        {
            if (node != null)
            {
                var bounds = ((NodeModel)node.Tag).Bounds;

                if (!bounds.IsEmpty && bounds.Width > 0 && bounds.Height >= 0)
                {
                    if (markerForm != null) markerForm.Dispose();
                    markerForm = new MarkerForm();
                    markerForm.ShowMarkerForm(bounds);
                }
            }
        }

    }
}
