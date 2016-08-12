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
using xServer.Core.Extensions;
using xServer.Core.Helper;

namespace xServer.Forms
{
    public partial class FrmRegistryEditor : Form
    {
        private readonly Client _connectClient;

        private readonly object locker = new object();

        #region Constants

        private const string PRIVILEGE_WARNING = "The client software is not running as administrator and therefore some functionality like Update, Create, Open and Delete may not work properly!";

        #endregion

        public FrmRegistryEditor(Client c)
        {
            _connectClient = c;
            _connectClient.Value.FrmRe = this;

            InitializeComponent();
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000; //WS_EX_COMPOSITED
                return cp;
            }
        }

        #region Main Form

        private void FrmRegistryEditor_Load(object sender, EventArgs e)
        {
            if (_connectClient.Value.AccountType != "Admin")
                MessageBox.Show(PRIVILEGE_WARNING, "Alert!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            
            if (_connectClient != null)
                this.Text = WindowHelper.GetWindowTitle("Registry Editor", _connectClient);

            // Signal client to retrive the root nodes (indicated by null)
            new xServer.Core.Packets.ServerPackets.DoLoadRegistryKey(null).Execute(_connectClient);
        }

        private void FrmRegistryEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_connectClient.Value != null)
                _connectClient.Value.FrmRe = null;
        }

        #endregion
        
        #region Helperfunctions

        public void ShowErrorMessage(string errorMsg)
        {
            this.Invoke((MethodInvoker)delegate
            {
                MessageBox.Show(errorMsg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            });
        }

        public void PerformClose()
        {
            this.Invoke((MethodInvoker)delegate
            {
                this.Close();
            });
        }

        #endregion

        #region TreeView Helperfunctions

        private void AddRootKey(RegSeekerMatch match)
        {
            TreeNode node = CreateNode(match.Key, match.Key, match.Data);
            node.Nodes.Add(new TreeNode());
            tvRegistryDirectory.Nodes.Add(node);
        }

        private TreeNode AddKeyToTree(TreeNode parent, RegSeekerMatch subKey)
        {
            TreeNode node = CreateNode(subKey.Key, subKey.Key, subKey.Data);
            parent.Nodes.Add(node);
            if (subKey.HasSubKeys)
                node.Nodes.Add(new TreeNode());
            return node;
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

        public void AddKeys(string rootName, RegSeekerMatch[] matches)
        {
            if (string.IsNullOrEmpty(rootName))
            {
                tvRegistryDirectory.Invoke((MethodInvoker)delegate
                {
                    tvRegistryDirectory.BeginUpdate();

                    foreach (var match in matches)
                        AddRootKey(match);

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
                            AddKeyToTree(parent, match);

                        parent.Expand();
                        tvRegistryDirectory.EndUpdate();
                    });
                }
            }
        }

        public void CreateNewKey(string rootKey, RegSeekerMatch match)
        {
            TreeNode parent = GetTreeNode(rootKey);

            tvRegistryDirectory.Invoke((MethodInvoker)delegate
            {
                TreeNode node = AddKeyToTree(parent, match);

                node.EnsureVisible();

                tvRegistryDirectory.SelectedNode = node;
                node.Expand();
                tvRegistryDirectory.LabelEdit = true;
                node.BeginEdit();
            });
        }

        public void RemoveKey(string rootKey, string subKey)
        {
            TreeNode parent = GetTreeNode(rootKey);

            if (parent.Nodes.ContainsKey(subKey)) {
                tvRegistryDirectory.Invoke((MethodInvoker)delegate
                {
                    parent.Nodes.RemoveByKey(subKey);
                });
            }
        }

        public void RenameKey(string rootKey, string oldName, string newName)
        {
            TreeNode parent = GetTreeNode(rootKey);

            if (parent.Nodes.ContainsKey(oldName))
            {
                tvRegistryDirectory.Invoke((MethodInvoker)delegate
                {
                    parent.Nodes[oldName].Text = newName;
                    parent.Nodes[oldName].Name = newName;

                    tvRegistryDirectory.SelectedNode = parent.Nodes[newName];
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
            string[] nodePath = path.Split(new char[] { '\\' });

            TreeNode lastNode = tvRegistryDirectory.Nodes[nodePath[0]];
            if (lastNode == null)
                return null;

            for (int i = 1; i < nodePath.Length; i++)
            {
                lastNode = lastNode.Nodes[nodePath[i]];
                if (lastNode == null)
                    return null;
            }
            return lastNode;
        }

        #endregion

        #region ListView Helpfunctions

        public void CreateValue(string keyPath, RegValueData value)
        {
            TreeNode key = GetTreeNode(keyPath);

            if (key != null )
            {
                lstRegistryValues.Invoke((MethodInvoker)delegate
                {
                    List<RegValueData> ValuesFromNode = ((RegValueData[])key.Tag).ToList();
                    ValuesFromNode.Add(value);
                    key.Tag = ValuesFromNode.ToArray();

                    if (tvRegistryDirectory.SelectedNode == key)
                    {
                        RegistryValueLstItem item = new RegistryValueLstItem(value);
                        lstRegistryValues.Items.Add(item);
                        //Unselect all
                        lstRegistryValues.SelectedIndices.Clear();
                        item.Selected = true;
                        lstRegistryValues.LabelEdit = true;
                        item.BeginEdit();
                    }

                    tvRegistryDirectory.SelectedNode = key;
                });
            }
        }

        public void DeleteValue(string keyPath, string valueName)
        {
            TreeNode key = GetTreeNode(keyPath);

            if (key != null)
            {
                lstRegistryValues.Invoke((MethodInvoker)delegate
                {
                    if (!RegValueHelper.IsDefaultValue(valueName))
                    {
                        //Remove the values that have the specified name
                        key.Tag = ((RegValueData[])key.Tag).Where(value => value.Name != valueName).ToArray();

                        if (tvRegistryDirectory.SelectedNode == key)
                            lstRegistryValues.Items.RemoveByKey(valueName);
                    }
                    else //Handle delete of default value
                    {
                        var regValue = ((RegValueData[])key.Tag).First(item => item.Name == valueName);
                        regValue.Data = null;

                        if(tvRegistryDirectory.SelectedNode == key)
                        {
                            var valueItem = lstRegistryValues.Items.Cast<RegistryValueLstItem>()
                                                         .SingleOrDefault(item => item.Name == valueName);
                            if (valueItem != null)
                                valueItem.Data = regValue.Kind.RegistryTypeToString(null);
                        }
                    }

                    tvRegistryDirectory.SelectedNode = key;

                });
            }
        }

        public void RenameValue(string keyPath, string oldName, string newName)
        {
            TreeNode key = GetTreeNode(keyPath);

            if (key != null)
            {
                lstRegistryValues.Invoke((MethodInvoker)delegate
                {
                    var value = ((RegValueData[])key.Tag).First(item => item.Name == oldName);
                    value.Name = newName;

                    if (tvRegistryDirectory.SelectedNode == key)
                    {
                        var valueItem = lstRegistryValues.Items.Cast<RegistryValueLstItem>()
                                                         .SingleOrDefault(item => item.Name == oldName);              
                        if (valueItem != null)
                            valueItem.RegName = newName;
                    }

                    tvRegistryDirectory.SelectedNode = key;
                });
            }
        }

        public void ChangeValue(string keyPath, RegValueData value)
        {
            TreeNode key = GetTreeNode(keyPath);

            if (key != null)
            {
                lstRegistryValues.Invoke((MethodInvoker)delegate
                {
                    var regValue = ((RegValueData[])key.Tag).First(item => item.Name == value.Name);
                    regValue.Data = value.Data;

                    if (tvRegistryDirectory.SelectedNode == key)
                    {
                        var valueItem = lstRegistryValues.Items.Cast<RegistryValueLstItem>()
                                                         .SingleOrDefault(item => item.Name == value.Name);
                        if (valueItem != null)
                            valueItem.Data = value.Kind.RegistryTypeToString(value.Data);
                    }

                    tvRegistryDirectory.SelectedNode = key;
                });
            }
        }

        private void UpdateLstRegistryValues(TreeNode node)
        {
            selectedStripStatusLabel.Text = node.FullPath;

            RegValueData[] ValuesFromNode = (RegValueData[])node.Tag;

            PopulateLstRegistryValues(ValuesFromNode);
        }

        private void PopulateLstRegistryValues(RegValueData[] values)
        {
            lstRegistryValues.BeginUpdate();
            lstRegistryValues.Items.Clear();

            //Sort values
            values = (
                from value in values
                orderby value.Name ascending
                select value
                ).ToArray();

            foreach (var value in values)
            {
                RegistryValueLstItem item = new RegistryValueLstItem(value);
                lstRegistryValues.Items.Add(item);
            }

            lstRegistryValues.EndUpdate();
        }

        #endregion

        #region tvRegistryDirectory Action

        private void tvRegistryDirectory_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (e.Label != null)
            {
                e.CancelEdit = true;

                if (e.Label.Length > 0)
                {
                    if (e.Node.Parent.Nodes.ContainsKey(e.Label))
                    {
                        MessageBox.Show("Invalid label. \nA node with that label already exists.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        e.Node.BeginEdit();
                    }
                    else
                    {
                        new xServer.Core.Packets.ServerPackets.DoRenameRegistryKey(e.Node.Parent.FullPath, e.Node.Name, e.Label).Execute(_connectClient);
                        tvRegistryDirectory.LabelEdit = false;
                    }
                }
                else
                {
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
            TreeNode parentNode = e.Node;

            // If nothing is there (yet).
            if (String.IsNullOrEmpty(parentNode.FirstNode.Name))
            {
                tvRegistryDirectory.SuspendLayout();
                parentNode.Nodes.Clear();

                new xServer.Core.Packets.ServerPackets.DoLoadRegistryKey(parentNode.FullPath).Execute(_connectClient);

                tvRegistryDirectory.ResumeLayout();

                e.Cancel = true;
            }
        }

        private void tvRegistryDirectory_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                //Bug fix with rightclick not working for selectednode
                tvRegistryDirectory.SelectedNode = e.Node;

                //Display the context menu
                Point pos = new Point(e.X, e.Y);
                CreateTreeViewMenuStrip();
                tv_ContextMenuStrip.Show(tvRegistryDirectory, pos);
            }
        }

        private void tvRegistryDirectory_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            UpdateLstRegistryValues(e.Node);
        }

        private void tvRegistryDirectory_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && GetDeleteState())
                deleteRegistryKey_Click(this, e);
        }

        #endregion

        #region ToolStrip and Contextmenu Helpfunctions

        private void CreateEditToolStrip()
        {
            this.modifyToolStripMenuItem1.Visible =
                this.modifyBinaryDataToolStripMenuItem1.Visible =
                    this.modifyNewtoolStripSeparator.Visible = lstRegistryValues.Focused;

            this.modifyToolStripMenuItem1.Enabled =
                this.modifyBinaryDataToolStripMenuItem1.Enabled = lstRegistryValues.SelectedItems.Count == 1;

            this.renameToolStripMenuItem2.Enabled = GetRenameState();
            this.deleteToolStripMenuItem2.Enabled = GetDeleteState();
        }

        private void  CreateTreeViewMenuStrip()
        {
            this.renameToolStripMenuItem.Enabled = tvRegistryDirectory.SelectedNode.Parent != null;

            this.deleteToolStripMenuItem.Enabled = tvRegistryDirectory.SelectedNode.Parent != null;
        }

        private void CreateListViewMenuStrip()
        {
            this.modifyToolStripMenuItem.Enabled =
                this.modifyBinaryDataToolStripMenuItem.Enabled = lstRegistryValues.SelectedItems.Count == 1;

            this.renameToolStripMenuItem1.Enabled = lstRegistryValues.SelectedItems.Count == 1 && !RegValueHelper.IsDefaultValue(lstRegistryValues.SelectedItems[0].Name);

            this.deleteToolStripMenuItem1.Enabled = tvRegistryDirectory.SelectedNode != null && lstRegistryValues.SelectedItems.Count > 0;
        }

        #endregion

        #region MenuStrip Action

        private void editToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            CreateEditToolStrip();
        }

        private void menuStripExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void menuStripDelete_Click(object sender, EventArgs e) {
            if(tvRegistryDirectory.Focused)
            {
                deleteRegistryKey_Click(this, e);
            }
            else if (lstRegistryValues.Focused) 
            {
                deleteRegistryValue_Click(this, e);
            }
        }

        private void menuStripRename_Click(object sender, EventArgs e)
        {
            if (tvRegistryDirectory.Focused)
            {
                renameRegistryKey_Click(this, e);
            }
            else if (lstRegistryValues.Focused)
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
                if (lstRegistryValues.GetItemAt(pos.X, pos.Y) == null)
                {
                    //Not on a item
                    lst_ContextMenuStrip.Show(lstRegistryValues, pos);
                }
                else
                {
                    //Clicked on a item
                    CreateListViewMenuStrip();
                    selectedItem_ContextMenuStrip.Show(lstRegistryValues, pos);
                }
            }
        }

        private void lstRegistryKeys_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            if (e.Label != null && tvRegistryDirectory.SelectedNode != null)
            {
                e.CancelEdit = true;
                int index = e.Item;

                if (e.Label.Length > 0)
                {
                    if (lstRegistryValues.Items.ContainsKey(e.Label))
                    {
                        MessageBox.Show("Invalid label. \nA node with that label already exists.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        lstRegistryValues.Items[index].BeginEdit();
                        return;
                    }

                    new xServer.Core.Packets.ServerPackets.DoRenameRegistryValue(tvRegistryDirectory.SelectedNode.FullPath, lstRegistryValues.Items[index].Name, e.Label).Execute(_connectClient);
                    
                    lstRegistryValues.LabelEdit = false;
                }
                else
                {
                    MessageBox.Show("Invalid label. \nThe label cannot be blank.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    lstRegistryValues.Items[index].BeginEdit();

                }
            }
            else
            {
                lstRegistryValues.LabelEdit = false;
            }
        }

        private void lstRegistryKeys_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && GetDeleteState())
                deleteRegistryValue_Click(this, e);
        }

        #endregion

        #region ContextMenu

        private void createNewRegistryKey_Click(object sender, EventArgs e)
        {
            if (!(tvRegistryDirectory.SelectedNode.IsExpanded) && tvRegistryDirectory.SelectedNode.Nodes.Count > 0)
            {
                //Subscribe (wait for node to expand)
                tvRegistryDirectory.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.createRegistryKey_AfterExpand);
                tvRegistryDirectory.SelectedNode.Expand();
            }
            else
            {
                new xServer.Core.Packets.ServerPackets.DoCreateRegistryKey(tvRegistryDirectory.SelectedNode.FullPath).Execute(_connectClient);
            }
        }

        private void deleteRegistryKey_Click(object sender, EventArgs e)
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

        private void renameRegistryKey_Click(object sender, EventArgs e)
        {
            tvRegistryDirectory.LabelEdit = true;
            tvRegistryDirectory.SelectedNode.BeginEdit();
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
            //Prompt user to confirm delete
            string msg = "Deleting certain registry values could cause system instability. Are you sure you want to permanently delete " + (lstRegistryValues.SelectedItems.Count == 1 ? "this value?": "these values?");
            string caption = "Confirm Value Delete";
            var answer = MessageBox.Show(msg, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (answer == DialogResult.Yes)
            {
                foreach (var item in lstRegistryValues.SelectedItems)
                {
                    if (item.GetType() == typeof(RegistryValueLstItem))
                    {
                        RegistryValueLstItem registyValue = (RegistryValueLstItem)item;
                        new xServer.Core.Packets.ServerPackets.DoDeleteRegistryValue(tvRegistryDirectory.SelectedNode.FullPath, registyValue.RegName).Execute(_connectClient);
                    }
                }
            }
        }

        private void renameRegistryValue_Click(object sender, EventArgs e)
        {
		    lstRegistryValues.LabelEdit = true;
		    lstRegistryValues.SelectedItems[0].BeginEdit();
        }

        private void modifyRegistryValue_Click(object sender, EventArgs e)
        {
            CreateModifyForm(false);
        }

        private void modifyBinaryDataRegistryValue_Click(object sender, EventArgs e)
        {
            CreateModifyForm(true);
        }

        #endregion

        #endregion

        #region Handlers

        private void createRegistryKey_AfterExpand(object sender, TreeViewEventArgs e)
        {
            if (e.Node == tvRegistryDirectory.SelectedNode)
            {
                createNewRegistryKey_Click(this, e);

                tvRegistryDirectory.AfterExpand -= new System.Windows.Forms.TreeViewEventHandler(this.createRegistryKey_AfterExpand);
            }
        }

        #endregion

        #region Help function

        private bool GetDeleteState()
        {
            if (lstRegistryValues.Focused)
                return lstRegistryValues.SelectedItems.Count > 0;
            else if (tvRegistryDirectory.Focused && tvRegistryDirectory.SelectedNode != null)
                return tvRegistryDirectory.SelectedNode.Parent != null;
            return false;
        }

        private bool GetRenameState()
        {
            if (lstRegistryValues.Focused)
                return lstRegistryValues.SelectedItems.Count == 1 && !RegValueHelper.IsDefaultValue(lstRegistryValues.SelectedItems[0].Name);
            else if (tvRegistryDirectory.Focused && tvRegistryDirectory.SelectedNode != null)
                return tvRegistryDirectory.SelectedNode.Parent != null;
            return false;
        }

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

        private void CreateModifyForm(bool isBinary)
        {
            string keyPath = tvRegistryDirectory.SelectedNode.FullPath;
            string name = lstRegistryValues.SelectedItems[0].Name;
            RegValueData value = ((RegValueData[])tvRegistryDirectory.SelectedNode.Tag).ToList().Find(item => item.Name == name);

            RegistryValueKind kind = isBinary ? RegistryValueKind.Binary : value.Kind;

            using (var frm = GetEditForm(keyPath, value, kind))
            {
                if (frm != null)
                    frm.ShowDialog();
            }
        }

        #endregion

    }
}
