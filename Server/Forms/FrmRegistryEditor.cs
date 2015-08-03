using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.Win32;
using xServer.Core.Networking;

namespace xServer.Forms
{
    public partial class FrmRegistryEditor : Form
    {
        // Notes: Use Microsoft.Win32.RegistryHive as a part of the packet to and from
        //        the client to represent the referenced top-level node.

        // To-Do: Migrate some of this code to a RegistryHelper.cs class.

        private readonly Client _connectClient;

        private RegistryHive currentClientHive = RegistryHive.ClassesRoot;

        public FrmRegistryEditor(Client c)
        {
            _connectClient = c;
            _connectClient.Value.FrmRe = this;

            InitializeComponent();

            // Request the top-level node being referenced.
            RegistryKey key = Registry.CurrentUser;

            TreeNode registryKeys = LoadRootSubKeyDirectories(key);
            if ((registryKeys != null) && registryKeys.Nodes.Count >= 0)
            {
                tvRegistryDirectory.Nodes.Add(registryKeys);
            }
        }

        private TreeNode LoadRootSubKeyDirectories(RegistryKey key, int depth = 4)
        {
            if (key != null && depth > 0)
            {
                TreeNode rootNode = new TreeNode(key.Name);

                try
                {
                    foreach (string subKeyName in key.GetSubKeyNames())
                    {
                        try
                        {
                            TreeNode subNode = new TreeNode(subKeyName);

                            using (RegistryKey SubKey = key.OpenSubKey(subKeyName))
                            {
                                if (SubKey != null)
                                {
                                    TreeNode lowerNode = LoadSubKeyDirectories(SubKey, depth--);

                                    // Load more sub-key directories, but with decrementing levels of depth.
                                    if (lowerNode != null)
                                    {
                                        subNode.Nodes.Add(lowerNode);
                                    }
                                }
                            }

                            rootNode.Nodes.Add(subNode);
                        }
                        catch
                        { }
                    }

                    return rootNode;
                }
                catch
                {
                    return rootNode;
                }
            }
            else
            {
                return null;
            }
        }

        private TreeNode LoadSubKeyDirectories(RegistryKey key, int depth = 4)
        {
            if (key != null && depth > 0)
            {
                TreeNode subNode = null;

                try
                {
                    subNode = new TreeNode(key.Name);

                    foreach (string subKeyName in key.GetSubKeyNames())
                    {
                        try
                        {
                            using (RegistryKey SubKey = key.OpenSubKey(subKeyName))
                            {
                                if (SubKey != null)
                                {
                                    TreeNode lowerNode = LoadSubKeyDirectories(SubKey, depth--);

                                    // Load more sub-key directories, but with decrementing levels of depth.
                                    if (lowerNode != null)
                                    {
                                        subNode.Nodes.Add(lowerNode);
                                    }
                                }
                            }
                        }
                        catch
                        { }
                    }
                }
                catch
                {
                    return null;
                }

                return subNode;
            }
            else
            {
                return null;
            }
        }

        private void PopulateLstRegistryKeys(RegistryKey[] keys)
        {
            for (int i = 0; i < lstRegistryKeys.Items.Count; i++)
            {
                RegistryKey key = lstRegistryKeys.Items[i].Tag as RegistryKey;
                if (key != null)
                {
                    key.Close();
                }
            }

            lstRegistryKeys.Clear();

            if (keys != null)
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
                foreach (object obj in (object[])tvRegistryDirectory.SelectedNode.Tag)
                {
                    RegistryKey key = obj as RegistryKey;
                    if (key != null)
                    {
                        KeysFromNode.Add(key);
                    }
                }

                PopulateLstRegistryKeys(KeysFromNode.ToArray());
            }
        }

        private void FrmRegistryEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_connectClient.Value.FrmRe != null)
                _connectClient.Value.FrmRe = null;
        }
    }
}