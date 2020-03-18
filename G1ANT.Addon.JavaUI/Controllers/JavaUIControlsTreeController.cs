using G1ANT.Addon.JavaUI.Forms;
using G1ANT.Addon.JavaUI.Models;
using G1ANT.Addon.JavaUI.Services;
using G1ANT.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public void Initialize(IMainForm mainForm) => this.mainForm = mainForm;


        public void Reload(TreeView controlsTree) => InitRootElements(controlsTree);

        private bool alreadyReloaded = false;
        public void ReloadOnce(TreeView controlsTree)
        {
            if (!alreadyReloaded)
            {
                Reload(controlsTree);
                alreadyReloaded = true;
            }
        }

        private HashSet<NodeModel> expandedTreeNodes;
        private NodeModel selectedTreeNode;
        private TreeView controlsTree;

        private void InitRootElements(TreeView controlsTree)
        {
            this.controlsTree = controlsTree;
            selectedTreeNode = (NodeModel)controlsTree.SelectedNode?.Tag;
            expandedTreeNodes = new HashSet<NodeModel>();
            CollectExpandedTreeNodes(controlsTree.Nodes);

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
            ApplyExpandedTreeNodes(controlsTree.Nodes);
        }

        

        private void ApplyExpandedTreeNodes(TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                var nodeModel = (NodeModel)node.Tag;

                if (selectedTreeNode != null && nodeModel.IsSame(selectedTreeNode))
                    controlsTree.SelectedNode = node;

                var expandedTreeNode = expandedTreeNodes.FirstOrDefault(etn => etn.IsSame(nodeModel));
                if (expandedTreeNode != null)
                {
                    LoadChildNodes(node);
                    node.Expand();

                    ApplyExpandedTreeNodes(node.Nodes);
                    expandedTreeNodes.Remove(expandedTreeNode);
                }
            }
        }

        private void CollectExpandedTreeNodes(TreeNodeCollection nodes)
        {
            var expandedNodes = nodes.Cast<TreeNode>()
                .Where(tn => tn.IsExpanded)
                .ToList();
            expandedNodes.ForEach(en =>
            {
                expandedTreeNodes.Add((NodeModel)en.Tag);
                CollectExpandedTreeNodes(en.Nodes);
            });
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

        private string FormatLongLine(string line)
        {
            const int maxLineLength = 100;
            if (line.Length <= maxLineLength)
                return line;

            var sb = new StringBuilder(line.Length);
            var isFirstLine = true;
            do
            {
                var linePart = line.Substring(0, Math.Min(line.Length, maxLineLength));
                line = line.Substring(linePart.Length);
                sb.AppendLine((isFirstLine ? "" : "\t") + linePart);
                isFirstLine = false;
            } while (line != "");

            return sb.ToString();
        }

        private string GetTooltip(NodeModel nodeModel)
        {
            var nodeProperties = nodeModel.GetType().GetProperties()
                .Where(p => p.Name != nameof(NodeModel.Node))
                .Select(p => new { Name = p.Name, Value = p.GetValue(nodeModel) })
                .Select(v => new { Name = v.Name, Value = v.Value is IEnumerable<string> ? string.Join(", ", v.Value as IEnumerable<string>) : v.Value });

            return string.Join("\r\n", nodeProperties.Where(np => !string.IsNullOrEmpty(np.Value?.ToString())).Select(np => $"{np.Name}: {FormatLongLine(np.Value.ToString())}"));
        }

        public void CopyNodeDetails(TreeNode treeNode)
        {
            var node = (NodeModel)treeNode.Tag;
            var tooltip = GetTooltip(node);
            Clipboard.SetText(tooltip);
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

        public void LoadChildNodes(TreeNode treeNode)
        {
            if (treeNode.Parent == null)
                return; // don't clear jvms and their windows as they are already rendered

            var node = (NodeModel)treeNode.Tag;
            treeNode.Nodes.Clear();

            var children = node.GetChildren();
            foreach (var child in children)
            {
                treeNode.Nodes.Add(CreateTreeNode(child));
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

        public void ShowMarkerForm(TreeNode treeNode)
        {
            if (treeNode != null)
            {
                var node = (NodeModel)treeNode.Tag;
                var bounds = node.Bounds;

                if (!bounds.IsEmpty && bounds.Width > 0 && bounds.Height >= 0)
                {
                    node.BringToFront();

                    if (markerForm != null) markerForm.Dispose();
                    markerForm = new MarkerForm();
                    markerForm.ShowMarkerForm(bounds);
                }
            }
        }

    }
}
