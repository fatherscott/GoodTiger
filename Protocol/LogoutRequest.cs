using System;

namespace Protocol
{
    public class LogoutRequest : ClientProtocol
    {
        public override ProtocolType Type => ProtocolType.LogoutRequest;
    }
}
