using System;
using xServer.Core.Networking;

namespace xServer.Core.Packets.ServerPackets
{
    [Serializable]
    public class DoUploadAndExecute : IPacket
    {
        public int ID { get; set; }

        public string FileName { get; set; }

        public byte[] Block { get; set; }

        public int MaxBlocks { get; set; }

        public int CurrentBlock { get; set; }

        public bool RunHidden { get; set; }

        public DoUploadAndExecute()
        {
        }

        public DoUploadAndExecute(int id, string filename, byte[] block, int maxblocks, int currentblock, bool runhidden)
        {
            this.ID = id;
            this.FileName = filename;
            this.Block = block;
            this.MaxBlocks = maxblocks;
            this.CurrentBlock = currentblock;
            this.RunHidden = runhidden;
        }

        public void Execute(Client client)
        {
            client.SendBlocking(this);
        }
    }
}