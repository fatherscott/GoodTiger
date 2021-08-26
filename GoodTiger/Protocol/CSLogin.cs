using Protocol;
using System.Threading.Tasks.Dataflow;

namespace GoodTiger
{
    public class CSLogin : Protocol.Base
    {
        public string UID { get; set; }
        public string Room { get; set; }
        public string NickName { get; set; }
        public BufferBlock<SocketBuffer> SendChan { get; set; }
    }
}
