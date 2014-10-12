using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Core;
using xRAT_2.Settings;

namespace xRAT_2.Forms
{
    public partial class frmProfileChooser : Form
    {
        public frmProfileChooser()
        {
            InitializeComponent();
        }

        private void frmProfileChooser_Load(object sender, EventArgs e)
        {
            XMLSettings.LastSelectedProfile = XMLSettings.ReadValue("LastSelectedProfile");
            String settingsDirPath = Path.Combine(Application.StartupPath, "Profiles\\");
            try
            {
                foreach (string profile in Directory.GetFiles(settingsDirPath))
                {
                    string[] profilearray = new string[2] { Path.GetFileNameWithoutExtension(profile), profile };
                    listViewProfileSelect.Items.Add(new ListViewItem(profilearray));
                }
                if (listViewProfileSelect.Items.Count >= 1)
                {
                    foreach (ListViewItem lvi in listViewProfileSelect.Items)
                    {
                        if (lvi.Text == XMLSettings.LastSelectedProfile)
                        {
                            listViewProfileSelect.EnsureVisible(lvi.Index);
                            listViewProfileSelect.Items[lvi.Index].Selected = true;
                            listViewProfileSelect.Select();
                        }
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void btnProfileSelect_Click(object sender, EventArgs e)
        {
            if (listViewProfileSelect.SelectedItems.Count > 0)
            {
                string selectedpath;
                selectedpath = listViewProfileSelect.SelectedItems[0].SubItems[0].Text;
                using (var frm = new frmBuilder(selectedpath))
                {
                    frm.ShowDialog();
                }
                XMLSettings.LastSelectedProfile = Path.GetFileNameWithoutExtension(selectedpath);
                XMLSettings.WriteValue("LastSelectedProfile", XMLSettings.LastSelectedProfile);
                this.Close();
            }
            else
            {
                MessageBox.Show("You have to select one profile!");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string profilename = "Default";
            InputBox.Show("Profilename", "Select a Profilename:", ref profilename);
            using (var frm = new frmBuilder(profilename))
            {
                frm.ShowDialog();
            }
            XMLSettings.LastSelectedProfile = profilename;
            XMLSettings.WriteValue("LastSelectedProfile", XMLSettings.LastSelectedProfile);
            this.Close();
        }
    }
}
