using Microsoft.Extensions.ObjectPool;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Protocol;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks.Dataflow;

namespace GoodTiger
{
    public class StateObject
    {
        public string UID { get; set; } = string.Empty;
        public Socket Socket { get; set; } = null;
        //public NetworkStream Strem { get; set; } = null;
        public SocketBuffer RecvBuffer { get; set; } = new SocketBuffer();
        public BufferBlock<SocketBuffer> SendChan { get; set; } = new BufferBlock<SocketBuffer>();
        public CancellationTokenSource RecvCancel { get; set; } = default;
        public CancellationTokenSource SendCancel = new CancellationTokenSource();
        public ObjectPool<SocketBuffer> SocketBufferPool { get; set; } = null;
        public BufferBlock<Protocol.Base> MainChan { get; set; } = null;
        public JsonSerializer JsonSerializer { get; set; } = null;
    }
}
