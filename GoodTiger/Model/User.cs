using Protocol;
using System.Threading.Tasks.Dataflow;

namespace GoodTiger.Model
{
    public class User : Base
    {
        public string UID { get; set; }
        public string Room { get; set; }
        public string NickName { get; set; }
        public BufferBlock<ClientProtocol> SendChan { get; set; }
        public ulong MemoryId { get; set; }
    }
}
