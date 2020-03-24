namespace Quasar.Common.Enums
{
    public enum ConnectionState : byte
    {
        Closed = 1,
        Listening = 2,
        SYN_Sent = 3,
        Syn_Recieved = 4,
        Established = 5,
        Finish_Wait_1 = 6,
        Finish_Wait_2 = 7,
        Closed_Wait = 8,
        Closing = 9,
        Last_ACK = 10,
        Time_Wait = 11,
        Delete_TCB = 12
    }
}
