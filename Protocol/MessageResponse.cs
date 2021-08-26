using System;
using System.Collections.Generic;
using System.Text;

namespace Protocol
{
    public class MessageResponse : Base
    {
        public string UID { get; set; }
        public string Message { get; set; }
    }
}
