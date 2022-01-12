using GoodTiger.Protocol;
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
        public ulong MemoryId { get; set; } = 0;
        public Socket Socket { get; set; } = null;

        public SocketBuffer RecvBuffer { get; set; } = new SocketBuffer();
        public CancellationTokenSource RecvCancel { get; set; } = default;

        public BufferBlock<global::Protocol.ClientProtocol> SendChan { get; set; } = new BufferBlock<global::Protocol.ClientProtocol>();
        public ObjectPool<SocketBuffer> SendSocketBufferPool { get; set; } = null;
        public CancellationTokenSource SendCancel { get; set; } = new CancellationTokenSource();

        public BufferBlock<ServerProtocol> MainChan { get; set; } = null;
        public BufferBlock<StateObject> StateObjectPool { get; set; } = null;

        public void Clear()
        {
            UID = string.Empty;
            MemoryId = 0;
        }

    }
}

