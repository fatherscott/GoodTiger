using System;
using System.Collections.Generic;
using System.Text;

namespace Protocol
{
    public class LoginResponse : ClientProtocol
    {
        public override ProtocolType Type => ProtocolType.LoginResponse;
        public string UID { get; set; }
        public string NickName { get; set; }
    }
}
