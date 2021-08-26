using Newtonsoft.Json;
using Protocol;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("GoodTiger Client Start!");

            JsonSerializer jsonSerializer = new JsonSerializer();

            IPHostEntry ipHostInfo = Dns.GetHostEntry("127.0.0.1");
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11000);

            Socket client = new Socket(ipAddress.AddressFamily, SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);

            await client.ConnectAsync(remoteEP);

            SocketBuffer socketBuffer = new SocketBuffer();

            using (LoginRequest login = new LoginRequest() { UID = "1", Room = "1", NickName = "tiger" })
            {
                await using var stream = new NetworkStream(client, false);
                await socketBuffer.Write(stream, login, jsonSerializer);
            }
            using (MessageRequest message = new MessageRequest() { Message = "Hello World" })
            {
                await using var stream = new NetworkStream(client, false);
                await socketBuffer.Write(stream, message, jsonSerializer);
            }

            client.Close();

            
        }
    }
}
