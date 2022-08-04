using GoodTiger.Model;
using Protocol;
using System.Threading.Tasks;

namespace GoodTiger.Protocol
{
    public abstract class ServerProtocol : Base
    {
        public long UID { get; set; }
        public ulong MemoryId { get; set; }
        public abstract Task Job(ServerMemory memory);
        public static ServerProtocol Get()
        {
            return null;
        }

        public const int PoolSize = 2;
    }
}
