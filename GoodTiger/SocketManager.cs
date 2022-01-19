using GoodTiger.Protocol;
using Microsoft.Extensions.ObjectPool;
using Newtonsoft.Json;
using Protocol;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace GoodTiger
{
    public partial class SocketManager
    {
        protected BufferBlock<StateObject> _stageObjectPool { get; set; } = new BufferBlock<StateObject>();
        protected CancellationTokenSource _recvCancel = new CancellationTokenSource();
        protected ObjectPool<SocketBuffer> _socketBufferPool { get; set; } = null;

        protected BufferBlock<ServerProtocol> _mainChan { get; set; } = new BufferBlock<ServerProtocol>();

        protected Task _mainProc { get; set; } = null;
        protected int _port { get; private set; } = 30000;
        public void Initialization(int port, int poolSize)
        {
            Initialization();

            _port = port;

            _socketBufferPool = ObjectPool.Create<SocketBuffer>();

            if (_stageObjectPool.Count == 0)
            {
                for (int i = 0; i < poolSize; i++)
                {
                    StateObject state = new StateObject();
                    state.RecvCancel = _recvCancel;
                    state.SendSocketBufferPool = _socketBufferPool;
                    state.MainChan = _mainChan;
                    state.StateObjectPool = _stageObjectPool;
                    _stageObjectPool.Post(state);
                }
            }

            _mainProc = Task.Run(async () => await MainProc());
        }

        public async Task StartListening(BufferBlock<Socket> serverSocektChan = null)
        {
            Socket listener = null;

            try
            {
                IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, _port);

                listener = new Socket(localEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                listener.Bind(localEndPoint);
                listener.Listen(100);

                if (serverSocektChan != null)
                {
                    await serverSocektChan.SendAsync(listener);
                }

                while (true)
                {
                    var state = await _stageObjectPool.ReceiveAsync();

                    state.Clear();

                    state.Socket = await listener.AcceptAsync();

                    try
                    {
#pragma warning disable CS4014
                        Task.Run(async () => await Recv(state));
#pragma warning restore CS4014
                    }
                    catch { }
                }
            }
            catch (Exception e)
            {
                Logger.Instance.Error("Exception", e);
            }
            finally
            {
                listener.Close();
            }

            _recvCancel.Cancel();

            await _mainChan.SendAsync(null);
            await Task.WhenAll(_mainProc);

        }
    }
}
