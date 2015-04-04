using ProtoBuf;

namespace xServer.Core.Packets.ServerPackets
{
    [ProtoContract]
    public class ShowMessageBox : IPacket
    {
        public ShowMessageBox()
        {
        }

        public ShowMessageBox(string caption, string text, string messageboxbutton, string messageboxicon)
        {
            Caption = caption;
            Text = text;
            MessageboxButton = messageboxbutton;
            MessageboxIcon = messageboxicon;
        }

        [ProtoMember(1)]
        public string Caption { get; set; }

        [ProtoMember(2)]
        public string Text { get; set; }

        [ProtoMember(3)]
        public string MessageboxButton { get; set; }

        [ProtoMember(4)]
        public string MessageboxIcon { get; set; }

        public void Execute(Client client)
        {
            client.Send<ShowMessageBox>(this);
        }
    }
}