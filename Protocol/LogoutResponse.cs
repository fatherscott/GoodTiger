using System;
using System.Collections.Generic;
using System.Text;

namespace Protocol
{
    public class LogoutResponse : ClientProtocol
    {
        public override ProtocolType Type => ProtocolType.LogoutResponse;
        public string UID { get; set; }
        public string NickName { get; set; }
    }
}
