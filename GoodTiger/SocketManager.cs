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
        protected ActionBlock<StateObject> _userState { get; set; } = null;
        protected CancellationTokenSource _recvCancel = new CancellationTokenSource();
        protected ObjectPool<SocketBuffer> _socketBufferPool { get; set; } = null;

        protected ObjectPool<MainObject> _mainObjectPool { get; set; } = null;
        protected BufferBlock<Protocol.Base> _mainChan { get; set; } = new BufferBlock<Protocol.Base>();
        protected JsonSerializer _jsonSerializer = new JsonSerializer();

        //Initialization
        public void Initialization(int poolSize)
        {
            _socketBufferPool = ObjectPool.Create<SocketBuffer>();
            _mainObjectPool = ObjectPool.Create<MainObject>();

            if (_recycling.Count == 0)
            {
                for (int i = 0; i < poolSize; i++)
                {
                    StateObject state = new StateObject();
                    state.RecvCancel = _recvCancel;
                    state.SocketBufferPool = _socketBufferPool;
                    state.MainChan = _mainChan;
                    state.JsonSerializer = _jsonSerializer;
                    _recycling.Post(state);
                }
            }

            //var maxDegreeOfParallelism = 100000;

            // Create an ActionBlock<int> that performs some work.
            _userState = new ActionBlock<StateObject>(Recv, new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = poolSize
            });
        }

        public async Task StartListening()
        {
            Socket listener = null;

            try
            {
                // Establish the local endpoint for the socket.  
                // The DNS name of the computer  
                // running the listener is "host.contoso.com".  
                //IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                //IPAddress ipAddress = ipHostInfo.AddressList[0];
                int port = 11000;
                IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, port);

                listener = new Socket(localEndPoint.AddressFamily, SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);

                listener.Bind(localEndPoint);
                listener.Listen(100);

                Console.WriteLine($"Bind IP:Any, Port:{port}");

                //server = new TcpListener(localEndPoint);

                // Start listening for client requests.
                //server.Start();

                while (true)
                {
                    var state = await _recycling.ReceiveAsync();
                    
                    state.UID = string.Empty;

                    state.Socket = await listener.AcceptAsync(state.Socket);

                    _userState.Post(state);
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                // Stop listening for new clients.
                listener.Close();
            }

            _recvCancel.Cancel();

            _userState.Complete();
            _userState.Completion.Wait();


            Console.WriteLine("\nHit enter to continue...");
            Console.Read();

        }
    }
}
