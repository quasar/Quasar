using System;
using System.Collections;

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

        public void Clear()
        {
            head.Clear();
        }
    }

    internal class BasicList : IEnumerable
    {
        /* Requirements:
         *   - Fast access by index
         *   - Immutable in the tail, so a node can be read (iterated) without locking
         *   - Lock-free tail handling must match the memory mode; struct for Node
         *     wouldn't work as "read" would not be atomic
         *   - Only operation required is append, but this shouldn't go out of its
         *     way to be inefficient
         *   - Assume that the caller is handling thread-safety (to co-ordinate with
         *     other code); no attempt to be thread-safe
         *   - Assume that the data is private; internal data structure is allowed to
         *     be mutable (i.e. array is fine as long as we don't screw it up)
         */
        private static readonly Node nil = new Node(null, 0);
        protected Node head = nil;

        public object this[int index]
        {
            get { return head[index]; }
        }

        public int Count
        {
            get { return head.Length; }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new NodeEnumerator(head);
        }

        public void CopyTo(Array array, int offset)
        {
            head.CopyTo(array, offset);
        }

        public int Add(object value)
        {
            return (head = head.Append(value)).Length - 1;
        }

        //public object TryGet(int index)
        //{
        //    return head.TryGet(index);
        //}
        public void Trim()
        {
            head = head.Trim();
        }

        public NodeEnumerator GetEnumerator()
        {
            return new NodeEnumerator(head);
        }

        internal int IndexOf(MatchPredicate predicate, object ctx)
        {
            return head.IndexOf(predicate, ctx);
        }

        internal int IndexOfString(string value)
        {
            return head.IndexOfString(value);
        }

        internal int IndexOfReference(object instance)
        {
            return head.IndexOfReference(instance);
        }

        internal bool Contains(object value)
        {
            foreach (var obj in this)
            {
                if (Equals(obj, value)) return true;
            }
            return false;
        }

        internal static BasicList GetContiguousGroups(int[] keys, object[] values)
        {
            if (keys == null) throw new ArgumentNullException("keys");
            if (values == null) throw new ArgumentNullException("values");
            if (values.Length < keys.Length)
                throw new ArgumentException("Not all keys are covered by values", "values");
            var outer = new BasicList();
            Group group = null;
            for (var i = 0; i < keys.Length; i++)
            {
                if (i == 0 || keys[i] != keys[i - 1])
                {
                    group = null;
                }
                if (group == null)
                {
                    group = new Group(keys[i]);
                    outer.Add(group);
                }
                group.Items.Add(values[i]);
            }
            return outer;
        }

        public struct NodeEnumerator : IEnumerator
        {
            private readonly Node node;
            private int position;

            internal NodeEnumerator(Node node)
            {
                position = -1;
                this.node = node;
            }

            void IEnumerator.Reset()
            {
                position = -1;
            }

            public object Current
            {
                get { return node[position]; }
            }

            public bool MoveNext()
            {
                var len = node.Length;
                return (position <= len) && (++position < len);
            }
        }

        internal sealed class Node
        {
            //public object TryGet(int index)
            //{
            //    return (index >= 0 && index < length) ? data[index] : null;
            //}
            private readonly object[] data;

            internal Node(object[] data, int length)
            {
                Helpers.DebugAssert((data == null && length == 0) ||
                                    (data != null && length > 0 && length <= data.Length));
                this.data = data;

                Length = length;
            }

            public object this[int index]
            {
                get
                {
                    if (index >= 0 && index < Length)
                    {
                        return data[index];
                    }
                    throw new ArgumentOutOfRangeException("index");
                }
                set
                {
                    if (index >= 0 && index < Length)
                    {
                        data[index] = value;
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException("index");
                    }
                }
            }

            public int Length { get; private set; }

            public void RemoveLastWithMutate()
            {
                if (Length == 0) throw new InvalidOperationException();
                Length -= 1;
            }

            public Node Append(object value)
            {
                object[] newData;
                var newLength = Length + 1;
                if (data == null)
                {
                    newData = new object[10];
                }
                else if (Length == data.Length)
                {
                    newData = new object[data.Length*2];
                    Array.Copy(data, newData, Length);
                }
                else
                {
                    newData = data;
                }
                newData[Length] = value;
                return new Node(newData, newLength);
            }

            public Node Trim()
            {
                if (Length == 0 || Length == data.Length) return this;
                var newData = new object[Length];
                Array.Copy(data, newData, Length);
                return new Node(newData, Length);
            }

            internal int IndexOfString(string value)
            {
                for (var i = 0; i < Length; i++)
                {
                    if (value == (string) data[i]) return i;
                }
                return -1;
            }

            internal int IndexOfReference(object instance)
            {
                for (var i = 0; i < Length; i++)
                {
                    if (instance == data[i]) return i;
                } // ^^^ (object) above should be preserved, even if this was typed; needs
                // to be a reference check
                return -1;
            }

            internal int IndexOf(MatchPredicate predicate, object ctx)
            {
                for (var i = 0; i < Length; i++)
                {
                    if (predicate(data[i], ctx)) return i;
                }
                return -1;
            }

            internal void CopyTo(Array array, int offset)
            {
                if (Length > 0)
                {
                    Array.Copy(data, 0, array, offset, Length);
                }
            }

            internal void Clear()
            {
                if (data != null)
                {
                    Array.Clear(data, 0, data.Length);
                }
                Length = 0;
            }
        }

        internal delegate bool MatchPredicate(object value, object ctx);

        internal sealed class Group
        {
            public readonly int First;
            public readonly BasicList Items;

            public Group(int first)
            {
                First = first;
                Items = new BasicList();
            }
        }
    }
}