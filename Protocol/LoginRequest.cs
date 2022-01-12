using System;

namespace Protocol
{
    public class LoginRequest : ClientProtocol
    {
        public override ProtocolType Type => ProtocolType.LoginRequest;
        public string UID { get; set; }
        public string Room { get; set; }
        public string NickName { get; set; }
    }
}
