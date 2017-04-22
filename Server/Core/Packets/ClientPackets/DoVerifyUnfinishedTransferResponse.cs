using System;
using xServer.Core.Networking;

namespace xServer.Core.Packets.ClientPackets
{
    [Serializable]
    public class DoVerifyUnfinishedTransferResponse : IPacket
    {
        public int[] TransferIDs { get; set; }

        public bool[] IsFile { get; set; }

        public DoVerifyUnfinishedTransferResponse(int[] transferIDs, bool[] isFile)
        {
            TransferIDs = transferIDs;
            IsFile = isFile;
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}
