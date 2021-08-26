using System;

namespace Protocol
{
    public class LoginRequest : Base
    {
        public string UID { get; set; }
        public string Room { get; set; }
        public string NickName { get; set; }
    }
}
