namespace ProtoBuf.Meta
{
    internal sealed class MutableList : BasicList
    {
        /*  Like BasicList, but allows existing values to be changed
         */

        public new object this[int index]
        {
            get { return head[index]; }
            set { head[index] = value; }
        }

        public void RemoveLast()
        {
            head.RemoveLastWithMutate();
        }
    }
}