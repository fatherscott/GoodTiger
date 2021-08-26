using Newtonsoft.Json;
using Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    public class Client
    {
        public static int UID = 0;

        public static async Task Work(JsonSerializer jsonSerializer)
        {
            try
            {
                IPHostEntry ipHostInfo = Dns.GetHostEntry("127.0.0.1");
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11000);

                Socket client = new Socket(ipAddress.AddressFamily, SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);

                await client.ConnectAsync(remoteEP);

                SocketBuffer socketBuffer = new SocketBuffer();

                var uid = Interlocked.Increment(ref UID);

                using (LoginRequest login = new LoginRequest() { UID = $"{uid}", Room = "1", NickName = $"tiger{uid}" })
                {
                    await using var stream = new NetworkStream(client, false);
                    await socketBuffer.Write(stream, login, jsonSerializer);
                }
                {
                    await using var stream = new NetworkStream(client, false);
                    var response = await socketBuffer.Read(stream, jsonSerializer);
                }

                for (int i = 0; i < 100; i++)
                {
                    using (MessageRequest message = new MessageRequest() { Message = $"Hello World {i}" })
                    {
                        await using var stream = new NetworkStream(client, false);
                        await socketBuffer.Write(stream, message, jsonSerializer);
                    }
                    {
                        await using var stream = new NetworkStream(client, false);
                        var response = await socketBuffer.Read(stream, jsonSerializer);
                    }
                }
                client.Close();
            }
            catch(Exception e)
            {
                Console.WriteLine($"{e.Message }, {e.StackTrace}");
            }
        }
    }
}
