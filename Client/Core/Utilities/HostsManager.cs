using System.Collections.Generic;
using xClient.Core.Data;

namespace xClient.Core.Utilities
{
    public class HostsManager
    {
        public bool IsEmpty { get { return _hosts.Count == 0; } }

        private readonly Queue<Host> _hosts = new Queue<Host>();

        public HostsManager(List<Host> hosts)
        {
            foreach(var host in hosts)
                _hosts.Enqueue(host);
        }

        public Host GetNextHost()
        {
            var temp = _hosts.Dequeue();
            _hosts.Enqueue(temp); // add to the end of the queue

            return temp;
        }
    }
}
