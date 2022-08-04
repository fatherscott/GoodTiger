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
        public long UID { get; set; } = 0;
        public ulong MemoryId { get; set; } = 0;
        public Socket Socket { get; set; } = null;

        public SocketBuffer RecvBuffer { get; set; } = new SocketBuffer();
        public CancellationTokenSource RecvCancel { get; set; } = default;

        public BufferBlock<global::Protocol.ClientProtocol> SendChan { get; set; } = new BufferBlock<global::Protocol.ClientProtocol>();
        public ObjectPool<SocketBuffer> SendSocketBufferPool { get; set; } = null;

        public BufferBlock<ServerProtocol> MainChan { get; set; } = null;
        public BufferBlock<StateObject> StateObjectPool { get; set; } = null;

        public void Clear()
        {
            UID = 0;
            MemoryId = 0;
            Socket = null;
        }

    }
}

