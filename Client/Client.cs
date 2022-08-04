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
        public static async Task TestClient(int uid)
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

                for (int i = 0; i < 9999999; i++)
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
                Console.WriteLine(e);
            }
        }
    }
}
