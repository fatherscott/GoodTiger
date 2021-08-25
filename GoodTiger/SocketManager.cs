using Microsoft.Extensions.ObjectPool;
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


        //Initialization
        public void Initialization(int poolSize)
        {
            _socketBufferPool = ObjectPool.Create<SocketBuffer>();

            if (_recycling.Count == 0)
            {
                for (int i = 0; i < poolSize; i++)
                {
                    StateObject state = new StateObject();
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
            TcpListener server = null;

            try
            {
                // Establish the local endpoint for the socket.  
                // The DNS name of the computer  
                // running the listener is "host.contoso.com".  
                //IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                //IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, 11000);

                server = new TcpListener(localEndPoint);

                // Start listening for client requests.
                server.Start();

                while(true)
                {
                    var state = await _recycling.ReceiveAsync();
                    state.RecvCancel = _recvCancel;
                    state.SocketBufferPool = _socketBufferPool;

                    state.Client = server.AcceptTcpClient();

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
                server.Stop();
            }

            _recvCancel.Cancel();

            _userState.Complete();
            _userState.Completion.Wait();


            Console.WriteLine("\nHit enter to continue...");
            Console.Read();

        }
    }
}
