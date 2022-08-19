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

        public void SetState(StateObject state)
        {
            UID = state.UID;
            MemoryId = state.MemoryId;
        }

        public bool Verify(User user)
        {
            if (user.MemoryId != MemoryId)
            {
                return false;
            }
            return true;
        }

        public const int PoolSize = 2;
    }
}
