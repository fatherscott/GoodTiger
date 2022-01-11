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
        protected BufferBlock<StateObject> _recycling { get; set; } = new BufferBlock<StateObject>();
        protected CancellationTokenSource _recvCancel = new CancellationTokenSource();
        protected ObjectPool<SocketBuffer> _socketBufferPool { get; set; } = null;

        protected BufferBlock<Base> _mainChan { get; set; } = new BufferBlock<Protocol.Base>();
        protected JsonSerializer _jsonSerializer = new JsonSerializer();

        protected Task _mainProc { get; set; } = null;
        protected int _port = 11000;
        public void Initialization(int port, int poolSize)
        {
            _port = port;
            _socketBufferPool = ObjectPool.Create<SocketBuffer>();

            if (_recycling.Count == 0)
            {
                for (int i = 0; i < poolSize; i++)
                {
                    StateObject state = new StateObject();
                    state.RecvCancel = _recvCancel;
                    state.SocketBufferPool = _socketBufferPool;
                    state.MainChan = _mainChan;
                    state.JsonSerializer = _jsonSerializer;
                    state.Recycling = _recycling;
                    _recycling.Post(state);
                }
            }

            _mainProc = Task.Run(async ()=> await MainProc());
        }

        public async Task StartListening(BufferBlock<Socket> serverSocektChan = null)
        {
            Socket listener = null;

            try
            {
                IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, _port);

                listener = new Socket(localEndPoint.AddressFamily, SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);

                listener.Bind(localEndPoint);
                listener.Listen(100);

                Logger.Instance.Info($"Bind IP:Any, Port:{_port}");

                if (serverSocektChan != null)
                {
                    await serverSocektChan.SendAsync(listener);
                }


                try
                {
                    while (true)
                    {
                        var state = await _recycling.ReceiveAsync();

                        state.UID = string.Empty;

                        state.Socket = await listener.AcceptAsync();
#pragma warning disable CS4014
                        Task.Run(async () => await Recv(state));
#pragma warning restore CS4014

                    }
                }
                catch (Exception e)
                {
                    Logger.Instance.Error("Exception", e);
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
