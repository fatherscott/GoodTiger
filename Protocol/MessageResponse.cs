using System;
using System.Collections.Generic;
using System.Text;

namespace Protocol
{
    public class MessageResponse : ClientProtocol
    {
        public override ProtocolType Type => ProtocolType.MessageResponse;
        public string UID { get; set; }
        public string Message { get; set; }
    }
}
