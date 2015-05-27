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

        public ShellCommandResponse(string output, bool _IsError = false)
        {
            this.Output = output;
            this.IsError = _IsError;
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}