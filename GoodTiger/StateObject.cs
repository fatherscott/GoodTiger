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
        public BufferBlock<Base> SendChan { get; set; } = new BufferBlock<Base>();
        public CancellationTokenSource RecvCancel { get; set; } = default;
        public ObjectPool<SocketBuffer> SocketBufferPool { get; set; } = null;
        public BufferBlock<Protocol.Base> MainChan { get; set; } = null;
        public JsonSerializer JsonSerializer { get; set; } = null;
        public BufferBlock<StateObject> Recycling { get; set; } = null;
        public CancellationTokenSource SendCancel { get; set; } = new CancellationTokenSource();
    }
}
