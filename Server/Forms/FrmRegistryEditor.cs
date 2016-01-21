using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
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
    }
}
