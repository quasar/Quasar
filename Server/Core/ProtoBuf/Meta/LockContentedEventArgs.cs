using System;

namespace ProtoBuf.Meta
{
    /// <summary>
    /// Contains the stack-trace of the owning code when a lock-contention scenario is detected
    /// </summary>
    public sealed class LockContentedEventArgs : EventArgs
    {
        private readonly string ownerStackTrace;

        internal LockContentedEventArgs(string ownerStackTrace)
        {
            this.ownerStackTrace = ownerStackTrace;
        }

        /// <summary>
        /// The stack-trace of the code that owned the lock when a lock-contention scenario occurred
        /// </summary>
        public string OwnerStackTrace
        {
            get { return ownerStackTrace; }
        }
    }
}