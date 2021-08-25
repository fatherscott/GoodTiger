using Microsoft.Extensions.ObjectPool;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks.Dataflow;

namespace GoodTiger
{
    public class StateObject
    {
        public TcpClient Client { get; set; } = null;
        public SocketBuffer RecvBuffer { get; set; } = new SocketBuffer();
        public BufferBlock<SocketBuffer> SendChan { get; set; } = new BufferBlock<SocketBuffer>();
        public CancellationTokenSource RecvCancel { get; set; } = default;

        public CancellationTokenSource SendCancel = new CancellationTokenSource();
        public ObjectPool<SocketBuffer> SocketBufferPool { get; set; } = null;

    }
}
