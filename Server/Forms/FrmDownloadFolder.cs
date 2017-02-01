using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using xServer.Enums;

namespace xServer.Forms
{
    public partial class FrmDownloadFolder : Form
    {
        public bool Cancelled { get; private set; }

        public string RootName
        {
            set {
                treeFolder.Nodes.Clear();
                treeFolder.Nodes.Add(value);
                treeFolder.Nodes[0].ImageIndex = 1;
            }
        }

        public string[] SelectedItems
        {
            get
            {
                var items = new List<string>();
                foreach(TreeNode node in GetCheckedNodes(treeFolder.Nodes[0]))
                    if (node.Checked)
                        items.Add(node.Text);

                return items.ToArray();
            }
        }

        public FrmDownloadFolder()
        {
            InitializeComponent();

            if (treeFolder.Nodes.Count >= 1)
                treeFolder.Nodes[0].Checked = true;
        }

        private void FrmDownloadFolder_Load(object sender, EventArgs e)
        {
            Cancelled = false;
        }

        private List<TreeNode> GetCheckedNodes(TreeNode parent)
        {
            var nodes = new List<TreeNode>();

            foreach (TreeNode node in parent.Nodes)
            {
                if(node.Checked)
                    nodes.Add(node);

                if (node.Nodes.Count > 0)
                    nodes.AddRange(GetCheckedNodes(node));
            }

            return nodes;
        }

        public void AddItemToFileBrowser(string name, string size, PathType type, int imageIndex,
           DateTime lastModificationDate, DateTime creationDate)
        {
            try
            {
                if (lastModificationDate == DateTime.MinValue)
                {
                    treeFolder.Invoke((MethodInvoker)delegate
                    {
                        var node = treeFolder.Nodes[0].Nodes.Add(name);
                        node.ImageIndex = imageIndex;
                    });
                }

                treeFolder.Invoke((MethodInvoker)delegate
                {
                    var node = treeFolder.Nodes[0].Nodes.Add(name);
                    node.ImageIndex = imageIndex;
                });
            }
            catch (InvalidOperationException)
            {
            }
        }

        private void treeFolder_AfterCheck(object sender, TreeViewEventArgs e)
        {
            CheckNodesRecursive(e.Node);
        }

        private void CheckNodesRecursive(TreeNode parentNode)
        {
            foreach (TreeNode node in parentNode.Nodes)
            {
                node.Checked = true;

                foreach (TreeNode childNode in node.Nodes)
                    CheckNodesRecursive(childNode);
            }
        }

        private int TotalNodeCount(TreeNode parentNode)
        {
            int count = parentNode.Nodes.Count;

            foreach (TreeNode node in parentNode.Nodes)
                count += TotalNodeCount(node);

            return count;
        }

        private void zIPSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeFolder.SelectedNode != null)
            {
                if (treeFolder.SelectedNode.Tag == null)
                {
                    treeFolder.SelectedNode.Text += " (ZIP)";
                    treeFolder.SelectedNode.Tag = "zip";
                }
                else
                {
                    treeFolder.SelectedNode.Text = treeFolder.SelectedNode.Text.Replace(" (ZIP)", null);
                    treeFolder.SelectedNode.Tag = null;
                }
            }
        }

        private void cancelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cancelled = true;
            Close();
        }
    }
}
