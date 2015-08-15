using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.Win32;
using xServer.Core.Networking;
using xServer.Core.Utilities;

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

            // By passing no search arguments, we tell the client to provide us with a response of its root keys.
            new xServer.Core.Packets.ServerPackets.DoLoadRegistryKey(null).Execute(_connectClient);
        }

        public void AddRootKey(RegSeekerMatch match)
        {
            tvRegistryDirectory.Invoke((MethodInvoker)delegate
            {
                TreeNode node = CreateNode(match.Key, match.Value, null);
                tvRegistryDirectory.Nodes.Add(node);

                node.Nodes.Add(new TreeNode());
            });
        }

        public void AddRootKey(RegistryKey key)
        {
            tvRegistryDirectory.Invoke((MethodInvoker)delegate
            {
                TreeNode node = CreateNode(key.Name, key.Name, key);
                tvRegistryDirectory.Nodes.Add(node);

                node.Nodes.Add(new TreeNode());
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

        public void AddKeyToTree(TreeNode parent, RegistryKeyEx subKey)
        {
            RegistryKey key = subKey.Key;
            TreeNode newNode = CreateNode(key.Name, subKey.Name, key);
            parent.Nodes.Add(newNode);

            // If it contains more nodes, prepare for additional keys to be added to the new node.
            if (key.SubKeyCount > 0)
            {
                newNode.Nodes.Add(new TreeNode());
            }
        }

        public  void PopulateLstRegistryKeys(RegistryKey[] keys)
        {
            // Clear the ListView.
            for (int i = 0; i < lstRegistryKeys.Items.Count; i++)
            {
                RegistryKey key = lstRegistryKeys.Items[i].Tag as RegistryKey;
                if (key != null)
                {
                    key.Close();
                }
            }

            lstRegistryKeys.Clear();

            // If the array is not null, we have usable data.
            if (keys != null && keys.Length > 0)
            {
                for (int i = 0; i < keys.Length; i++)
                {
                    // To-Do: Use a custom ListViewItem for a better style.
                    lstRegistryKeys.Items.Add(new ListViewItem() { Text = keys[i].Name, Tag = keys[i] });
                }
            }
        }

        private void tvRegistryDirectory_Click(object sender, EventArgs e)
        {
            if ((tvRegistryDirectory.SelectedNode != null) && (tvRegistryDirectory.SelectedNode.Tag != null))
            {
                List<RegistryKey> KeysFromNode = new List<RegistryKey>();
                
                // Send a request for registry keys on this node.

                // ToDo: (After 

                // ToDo: Move the code below in a spot that the RegistryHandler will make use of
                if (KeysFromNode.Count < 1)
                {
                    // If there are valid keys to use from the selected TreeNode, populate the ListView with it.
                    PopulateLstRegistryKeys(KeysFromNode.ToArray());
                }
                else
                {
                    // If there aren't, send null to clear the ListView.
                    PopulateLstRegistryKeys(null);
                }
            }
            else
            {
                // It is likely that the user clicked on either an empty direction or an invalid RegistryKey.
                // Sending null to this method will clear the ListView.
                PopulateLstRegistryKeys(null);
            }
        }

        private void FrmRegistryEditor_Load(object sender, EventArgs e)
        {
            // Send packet to obtain the root nodes.
        }

        private void FrmRegistryEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_connectClient.Value != null)
                _connectClient.Value.FrmRe = null;
        }

        private void tvRegistryDirectory_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            // After the Tree Node label text is edited, 
        }

        private void tvRegistryDirectory_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }

        private void tvRegistryDirectory_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            // Before expansion of the node, prepare the first node with RegistryKeys.
            TreeNode parentNode = e.Node;

            // If nothing is there (yet).
            if (parentNode.FirstNode.Tag == null)
            {
                try
                {
                    tvRegistryDirectory.SuspendLayout();

                    // Send a packet to retrieve the data to use for the nodes.

                    parentNode.Nodes.Clear();
                    RegistryKey key = parentNode.Tag as RegistryKey;

                    if (key != null)
                    {
                        // Get sub-keys, sort them, then add them to the tree.
                        
                    }
                }
                finally
                {
                    ResumeLayout();
                }
            }
        }

        private void tvRegistryDirectory_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void tvRegistryDirectory_MouseUp(object sender, MouseEventArgs e)
        {

        }

        private void tvRegistryDirectory_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {

        }
    }
}