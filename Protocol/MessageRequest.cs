using System;
using System.Collections.Generic;
using System.Text;

namespace Protocol
{
    public class MessageRequest : ClientProtocol
    {
        public override ProtocolType Type => ProtocolType.MessageRequest;
        public string Message { get; set; }
    }
}
