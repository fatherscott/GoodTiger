using GoodTiger.Protocol;
using Microsoft.Extensions.ObjectPool;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Protocol;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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

        public BufferBlock<ClientProtocol> SendChan { get; set; } = new BufferBlock<ClientProtocol>();
        public ObjectPool<SocketBuffer> SendSocketBufferPool { get; set; } = null;

        public BufferBlock<ServerProtocol> MainChan { get; set; } = null;
        public BufferBlock<StateObject> StateObjectPool { get; set; } = null;

        public AutoResetEvent MainChanExit = new AutoResetEvent(false);

        public async Task Clear()
        {
            UID = 0;
            MemoryId = 0;
            Socket = null;
            while (SendChan.Count > 0)
            {
                var packet = await SendChan.ReceiveAsync();
                packet?.Return();
            }
            SendChan = new BufferBlock<ClientProtocol>();
        }

    }
}

