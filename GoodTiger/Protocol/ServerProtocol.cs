using GoodTiger.Model;
using Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodTiger.Protocol
{
    public abstract class ServerProtocol : Base
    {
        public string UID { get; set; }
        public ulong MemoryId { get; set; }
        public abstract Task Job(ServerMemory memory);
    }
}
