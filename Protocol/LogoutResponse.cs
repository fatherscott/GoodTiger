using System;
using System.Collections.Generic;
using System.Text;

namespace Protocol
{
    public class LogoutResponse : Base
    {
        public string UID { get; set; }
        public string NickName { get; set; }
    }
}
