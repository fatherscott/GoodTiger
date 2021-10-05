using Microsoft.Extensions.ObjectPool;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Protocol;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace GoodTiger
{
    public partial class SocketManager
    {
        protected BufferBlock<StateObject> _recycling { get; set; } = new BufferBlock<StateObject>();
        //protected ActionBlock<StateObject> _userState { get; set; } = null;
        protected CancellationTokenSource _recvCancel = new CancellationTokenSource();
        protected ObjectPool<SocketBuffer> _socketBufferPool { get; set; } = null;

        protected BufferBlock<Base> _mainChan { get; set; } = new BufferBlock<Protocol.Base>();
        protected JsonSerializer _jsonSerializer = new JsonSerializer();

        protected Task _mainProc { get; set; } = null;

        //Initialization
        public void Initialization(int poolSize)
        {
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

            //_userState = new ActionBlock<StateObject>(Recv, new ExecutionDataflowBlockOptions
            //{
            //    MaxDegreeOfParallelism = poolSize
            //});

        }

        public async Task StartListening()
        {
            Socket listener = null;

            try
            {
                int port = 11000;
                IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, port);

                listener = new Socket(localEndPoint.AddressFamily, SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);

                listener.Bind(localEndPoint);
                listener.Listen(100);

                Console.WriteLine($"Bind IP:Any, Port:{port}");


                try
                {
                    while (true)
                    {
                        var state = await _recycling.ReceiveAsync();

                        state.UID = string.Empty;

                        state.Socket = await listener.AcceptAsync();

                        Task.Run(async () => await Recv(state)); 

                        //await _userState.SendAsync(state);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception: {0}", e);
                }
              
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e);
            }
            finally
            {
                listener.Close();
            }

            _recvCancel.Cancel();

            //_userState.Complete();
            //await _userState.Completion;

            await _mainChan.SendAsync(null);
            await Task.WhenAll(_mainProc);

            Console.WriteLine("\nHit enter to continue...");
            Console.Read();

        }
    }
}
