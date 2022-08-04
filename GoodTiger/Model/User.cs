using Protocol;
using System.Threading.Tasks.Dataflow;

namespace GoodTiger.Model
{
    public class User : Base
    {
        public long UID { get; set; }
        public long Room { get; set; }
        public char[] NickName = new char[32];
        public BufferBlock<ClientProtocol> SendChan { get; set; }
        public ulong MemoryId { get; set; }
    }
}
