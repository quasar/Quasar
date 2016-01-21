using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using xServer.Controls;
using xServer.Core.Networking;
using xServer.Core.Registry;

namespace xServer.Forms
{
    public partial class FrmRegistryEditor : Form
    {
        private readonly Client _connectClient;

        private readonly object locker = new object();

        public FrmRegistryEditor(Client c)
        {
            _connectClient = c;
            _connectClient.Value.FrmRe = this;

            InitializeComponent();
        }

        #region Main Form

        private void FrmRegistryEditor_Load(object sender, EventArgs e)
        {
            //Check if user is not currently running as administrator
            if (_connectClient.Value.AccountType != "Admin")
            {
                //Prompt user of not being admin, probably change need clearer statement (TODO)
                string msg = "The client software is not running as administrator and therefore some functionality like Update, Create, Open and Delete my not work properly!";
                string caption = "Alert!";
                MessageBox.Show(msg, caption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            // Signal client to retrive the root nodes (indicated by null)
            new xServer.Core.Packets.ServerPackets.DoLoadRegistryKey(null).Execute(_connectClient);
        }

        private void FrmRegistryEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_connectClient.Value != null)
                _connectClient.Value.FrmRe = null;
        }

        #endregion

        #region TreeView Helperfunctions

        public void AddRootKey(RegSeekerMatch match)
        {
            tvRegistryDirectory.Invoke((MethodInvoker)delegate
            {
                TreeNode node = CreateNode(match.Key, match.Key, match.Data);
                node.Nodes.Add(new TreeNode());
                tvRegistryDirectory.Nodes.Add(node);
            });
        }

        private TreeNode CreateNode(string key, string text, object tag)
        {
            return new TreeNode()
            {
                Text = text,
                Name = key,
                Tag = tag
            };
        }

        public void AddKeysToTree(string rootName, RegSeekerMatch[] matches)
        {
            if (string.IsNullOrEmpty(rootName))
            {
                // Root key
                foreach (var match in matches)
                {
                    AddRootKey(match);
                }

                tvRegistryDirectory.Invoke((MethodInvoker)delegate
                {
                    tvRegistryDirectory.SelectedNode = tvRegistryDirectory.Nodes[0];
                });

            }
            else
            {

                TreeNode parent = GetParentTreeNode(rootName);

                // If the parent is null, it should be a root node.
                if (parent == null)
                {
                    //Error incorrect format
                    return;
                }
                else
                {
                    tvRegistryDirectory.Invoke((MethodInvoker)delegate
                    {
                        foreach (var match in matches)
                        {
                            //This will execute in the form thread
                            TreeNode node = CreateNode(match.Key, match.Key, match.Data);
                            if (match.HasSubKeys)
                            {
                                node.Nodes.Add(new TreeNode());
                            }
                            parent.Nodes.Add(node);
                        }
                        parent.Expand();
                    });
                }
            }
        }

        public void AddKeyToTree(string rootKey, RegSeekerMatch match)
        {
            TreeNode parent = GetParentTreeNode(rootKey);

            tvRegistryDirectory.Invoke((MethodInvoker)delegate
            {
                //This will execute in the form thread
                TreeNode node = CreateNode(match.Key, match.Key, match.Data);
                if (match.HasSubKeys)
                {
                    node.Nodes.Add(new TreeNode());
                }
                parent.Nodes.Add(node);
                if (!parent.IsExpanded)
                {
                    tvRegistryDirectory.SelectedNode = parent;
                    tvRegistryDirectory.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.specialCreateRegistryKey_AfterExpand);
                    parent.Expand();
                }
                else
                {
                    tvRegistryDirectory.SelectedNode = node;
                    tvRegistryDirectory.LabelEdit = true;
                    node.BeginEdit();
                }
            });
        }

        public void RemoveKeyFromTree(string rootKey, string subKey)
        {
            TreeNode parent = GetParentTreeNode(rootKey);

            //Error key does not exists
            if (!parent.Nodes.ContainsKey(subKey))
                return;

            tvRegistryDirectory.Invoke((MethodInvoker)delegate
            {
                parent.Nodes.RemoveByKey(subKey);
            });

        }

        public void RenameKeyFromTree(string rootKey, string oldName, string newName)
        {
            TreeNode parent = GetParentTreeNode(rootKey);

            //Error the key does not exist
            if (!parent.Nodes.ContainsKey(oldName))
                return;

            int index = parent.Nodes.IndexOfKey(oldName);

            //Temp - Should not be neccesary (only need to confirm the add)
            tvRegistryDirectory.Invoke((MethodInvoker)delegate
            {
                parent.Nodes[index].Text = newName;
                parent.Nodes[index].Name = newName;
            });
        }

        /// <summary>
        /// Using the RegSeekerMatch's name, obtain the parent TreeNode of the match, creating
        /// the TreeNodes if necessary.
        /// </summary>
        /// <param name="match">The match from which we obtain the corresponding TreeNode from.</param>
        /// <returns>Null if an invalid name is passed; The parent TreeNode for non-root matches; Returns
        /// itself if it is a root match.</returns>
        private TreeNode GetParentTreeNode(string rootName)
        {
            string[] nodePath = null;
            if (rootName.Contains("\\"))
            {
                // It might not be a root node.
                nodePath = rootName.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);

                // Only one valid node. Probably malformed or a root node.
                if (nodePath.Length < 2)
                {
                    return null;
                }
            }
            else
            {
                //Is a root node
                nodePath = new string[] { rootName };
            }

            // Keep track of the last node to reference for traversal.
            TreeNode lastNode = null;

            // If the TreeView contains the first element in the node path, we
            // won't have to create it. Otherwise, create it then set the last node
            // to serve as our reference point.
            if (tvRegistryDirectory.Nodes.ContainsKey(nodePath[0]))
            {
                lastNode = tvRegistryDirectory.Nodes[nodePath[0]];
            }
            else
            {
                // This node does not exist in the TreeView. Create it then add it.
                lastNode = CreateNode(nodePath[0], nodePath[0], null);
                tvRegistryDirectory.Invoke((MethodInvoker)delegate
                {
                    tvRegistryDirectory.Nodes.Add(lastNode);
                });

            }

            // Go through the rest of the node path (leave last one, is the one to add).
            for (int i = 1; i < nodePath.Length; i++)
            {
                // If the last node does have this entry in the path, just set
                // the last node to the existing entry.
                if (lastNode.Nodes.ContainsKey(nodePath[i]))
                {
                    lastNode = lastNode.Nodes[nodePath[i]];
                }
                else
                {
                    // If the last node does not contain the next item in the path,
                    // create the node and add it to the path.
                    TreeNode newNode = CreateNode(nodePath[i], nodePath[i], null);

                    tvRegistryDirectory.Invoke((MethodInvoker)delegate
                    {
                        lastNode.Nodes.Add(newNode);
                    });

                    lastNode = newNode;
                }
            }

            return lastNode;
        }

        #endregion

        #region Popup actions

        public void ShowErrorMessage(string errorMsg)
        {
            this.Invoke((MethodInvoker)delegate
            {
                MessageBox.Show(errorMsg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            });

        }

        #endregion

        #region ListView Helpfunctions

        public void PopulateLstRegistryKeys(List<RegValueData> values)
        {
            lstRegistryKeys.Items.Clear();

            // If the array is not null, we have usable data.
            if (values != null && values.Count > 0)
            {
                foreach (var value in values)
                {
                    // To-Do: Use a custom ListViewItem for a better style. (Maybe add the imageList to it?)
                    RegistryValueLstItem item = new RegistryValueLstItem(value.Name, value.Type, value.Data);
                    item.ImageIndex = GetRegistryValueImgIndex(value.Type);
                    lstRegistryKeys.Items.Add(item);
                }
            }
        }

        #endregion

        #region tvRegistryDirectory Action

        private void tvRegistryDirectory_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            //No need to edit if it is null
            if (e.Label != null)
            {
                //Prevent the change of the label
                e.CancelEdit = true;

                if (e.Label.Length > 0)
                {
                    foreach (TreeNode node in e.Node.Parent.Nodes)
                    {
                        if (node.Text == e.Label && node != e.Node)
                        {
                            //Prompt error
                            MessageBox.Show("Invalid label. \nA node with that label already exists.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            e.Node.BeginEdit();
                            return;
                        }
                    }

                    //Normal rename action
                    //Perform Rename action
                    new xServer.Core.Packets.ServerPackets.DoRenameRegistryKey(e.Node.Parent.FullPath, e.Node.Name, e.Label).Execute(_connectClient);

                    tvRegistryDirectory.LabelEdit = false;
                }
                else
                {
                    //Prompt error
                    MessageBox.Show("Invalid label. \nThe label cannot be blank.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Node.BeginEdit();

                }
            }
        }

        private void tvRegistryDirectory_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            // Before expansion of the node, prepare the first node with RegistryKeys.
            TreeNode parentNode = e.Node;

            // If nothing is there (yet).
            if (String.IsNullOrEmpty(parentNode.FirstNode.Name))
            {
                try
                {
                    tvRegistryDirectory.SuspendLayout();
                    parentNode.Nodes.Clear();

                    // Send a packet to retrieve the data to use for the nodes.
                    new xServer.Core.Packets.ServerPackets.DoLoadRegistryKey(parentNode.FullPath).Execute(_connectClient);
                }
                finally
                {
                    tvRegistryDirectory.ResumeLayout();
                }
                //Cancel expand
                e.Cancel = true;
            }
        }

        private void tvRegistryDirectory_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if ((e.Node.Tag != null))
            {
                selectedStripStatusLabel.Text = e.Node.FullPath;
                tvRegistryDirectory.SelectedNode = e.Node;

                List<RegValueData> ValuesFromNode = null;
                if (e.Node.Tag.GetType() == typeof(List<RegValueData>))
                {
                    ValuesFromNode = (List<RegValueData>)e.Node.Tag;
                }

                PopulateLstRegistryKeys(ValuesFromNode);
            }
            else
            {
                // It is likely that the user clicked on either an empty direction or an invalid RegistryKey.
                // Clear the ListView.
                PopulateLstRegistryKeys(null);
            }

            /* Enable delete and rename if not root node */
            this.deleteToolStripMenuItem.Enabled = tvRegistryDirectory.SelectedNode.Parent != null;
            this.renameToolStripMenuItem.Enabled = tvRegistryDirectory.SelectedNode.Parent != null;

            if (e.Button == MouseButtons.Right)
            {
                Point pos = new Point(e.X, e.Y);
                contextMenuStrip.Show(tvRegistryDirectory, pos);
            }
        }

        #endregion

        #region ContextMenu

        private void createNewRegistryKey_Click(object sender, EventArgs e)
        {
            if (tvRegistryDirectory.SelectedNode != null)
            {
                if (!(tvRegistryDirectory.SelectedNode.IsExpanded) && tvRegistryDirectory.SelectedNode.Nodes.Count > 0)
                {
                    //Subscribe
                    tvRegistryDirectory.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.createRegistryKey_AfterExpand);
                    tvRegistryDirectory.SelectedNode.Expand();
                }
                else
                {
                    //Try to create a new subkey
                    new xServer.Core.Packets.ServerPackets.DoCreateRegistryKey(tvRegistryDirectory.SelectedNode.FullPath).Execute(_connectClient);
                }
            }
        }

        private void deleteRegistryKey_Click(object sender, EventArgs e)
        {
            if (tvRegistryDirectory.SelectedNode != null && tvRegistryDirectory.SelectedNode.Parent != null)
            {
                //Prompt user to confirm delete
                string msg = "Are you sure you want to permanently delete this key and all of its subkeys?";
                string caption = "Confirm Key Delete";
                var answer = MessageBox.Show(msg, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (answer == DialogResult.Yes)
                {
                    string parentPath = tvRegistryDirectory.SelectedNode.Parent.FullPath;

                    new xServer.Core.Packets.ServerPackets.DoDeleteRegistryKey(parentPath, tvRegistryDirectory.SelectedNode.Name).Execute(_connectClient);
                }
            }
        }

        private void renameRegistryKey_Click(object sender, EventArgs e)
        {
            if (tvRegistryDirectory.SelectedNode != null)
            {
                tvRegistryDirectory.LabelEdit = true;
                tvRegistryDirectory.SelectedNode.BeginEdit();
            }
        }

        #endregion

        #region Handlers

        private void createRegistryKey_AfterExpand(object sender, TreeViewEventArgs e)
        {
            if (e.Node == tvRegistryDirectory.SelectedNode)
            {
                //Trigger a click
                createNewRegistryKey_Click(this, e);

                //Unsubscribe
                tvRegistryDirectory.AfterExpand -= new System.Windows.Forms.TreeViewEventHandler(this.createRegistryKey_AfterExpand);
            }
        }

        ////A special case for when the node was empty and add was performed before expand
        private void specialCreateRegistryKey_AfterExpand(object sender, TreeViewEventArgs e)
        {
            if (e.Node == tvRegistryDirectory.SelectedNode)
            {
                tvRegistryDirectory.SelectedNode = tvRegistryDirectory.SelectedNode.FirstNode;
                tvRegistryDirectory.LabelEdit = true;

                tvRegistryDirectory.SelectedNode.BeginEdit();

                //Unsubscribe
                tvRegistryDirectory.AfterExpand -= new System.Windows.Forms.TreeViewEventHandler(this.specialCreateRegistryKey_AfterExpand);
            }
        }

        #endregion

        #region HelpFunctions

        private int GetRegistryValueImgIndex(string type)
        {
            switch (type)
            {
                case "REG_MULTI_SZ":
                case "REG_SZ":
                case "REG_EXPAND_SZ":
                    return 0;
                case "REG_BINARY":
                case "REG_DWORD":
                case "REG_QWORD":
                default:
                    return 1;
            }
        }

        #endregion
    }
}
