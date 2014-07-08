namespace Core.Packets
{
    public interface IPacket 
    {
        void Execute(Client client);
    }
}
