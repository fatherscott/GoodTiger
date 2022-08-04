using GoodTiger;
using Newtonsoft.Json;
using NUnit.Framework;
using Protocol;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace BasicTest
{
    public class ServerTest
    {
        [SetUp]
        public void Setup()
        {
        }

        protected BufferBlock<Socket> _serverSocektChan { get; set; } = new BufferBlock<Socket>();

        public async Task TestClient(int uid)
        {
            try
            {
                IPHostEntry ipHostInfo = Dns.GetHostEntry("127.0.0.1");
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11000);

                Socket client = new Socket(ipAddress.AddressFamily, SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);

                await client.ConnectAsync(remoteEP);

                SocketBuffer socketBuffer = new SocketBuffer();

                LoginRequest login = LoginRequest.Get() as LoginRequest;
                login.UID = uid;
                login.Room = 1;
                login.NickName = $"tiger{uid}".ToCharArray();
               
                {
                    await using var stream = new NetworkStream(client, false);
                    await socketBuffer.Write(stream, login);
                    login.Return();
                }
                {
                    await using var stream = new NetworkStream(client, false);
                    var response = await socketBuffer.Read(stream);
                }

                MessageRequest message = MessageRequest.Get() as MessageRequest;

                for (int i = 0; i < 100; i++)
                {
                    message.Message = $"Hello World {i}".ToCharArray();
                    {
                        await using var stream = new NetworkStream(client, false);
                        await socketBuffer.Write(stream, message);
                    }
                    {
                        await using var stream = new NetworkStream(client, false);
                        var response = await socketBuffer.Read(stream);
                    }
                }
                client.Close();
            }
            catch (Exception e)
            {
                Assert.Fail($"{e.Message }, {e.StackTrace}");
            }
        }

        [Test]
        public async Task login_message_test()
        {
            SocketManager manager = new SocketManager();
            manager.Initialization(11000, 1000);
            var server = Task.Run(async () =>
                await manager.StartListening(_serverSocektChan)
            );

            Socket listener = null;

            try
            {
                listener = await _serverSocektChan.ReceiveAsync(TimeSpan.FromSeconds(5));
            }
            catch (Exception e)
            {
                Assert.Fail($"{e.Message }, {e.StackTrace}");
            }

            Assert.IsTrue(listener != null);

            await Task.Delay(1000);

            for(int i = 0; i < 1; i++)
            {
                await TestClient(i);
            }

            listener.Close();
            await Task.WhenAll(server);
        }
    }
}