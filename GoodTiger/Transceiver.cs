using GoodTiger.Parse;
using Protocol;
using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace GoodTiger
{
    public partial class SocketManager
    {

        public async Task Send(StateObject stateObject)
        {
            try
            {
                while (await stateObject.SendChan.OutputAvailableAsync(stateObject.SendCancel.Token))
                {
                    var protocol = await stateObject.SendChan.ReceiveAsync();

                    if (protocol == null)
                    {
                        return;
                    }

                    await using var Strem = new NetworkStream(stateObject.Socket);

                    var buffer = stateObject.SendSocketBufferPool.Get();
                    await buffer.Write(Strem, protocol, stateObject.SendCancel.Token);
                    stateObject.SendSocketBufferPool.Return(buffer);

                    protocol.Dispose();
                }
            }
            catch (Exception)
            {
            }
        }

        public async Task Recv(StateObject stateObject)
        {
            var sendTask = Task.Run(async () => await Send(stateObject));
            try
            {
                while (true)
                {
                    await using var strem = new NetworkStream(stateObject.Socket, false);
                    var obj = await stateObject.RecvBuffer.Read(strem, stateObject.RecvCancel.Token);

                    if (obj != null)
                    {
                        if (!await Parse(stateObject, obj))
                        {
                            throw new Exception("Parsing failed");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.Message}");
            }

            if (!string.IsNullOrEmpty(stateObject.UID))
            {
                CSLogout logout = new CSLogout
                {
                    UID = stateObject.UID
                };
                await stateObject.MainChan.SendAsync(logout);
            }

            stateObject.SendCancel.Cancel();
            await stateObject.SendChan.SendAsync(null);
            await Task.WhenAll(sendTask);

            Logger.Instance.Trace($"exit {stateObject.UID}");

            stateObject.Socket.Close();

            await stateObject.StateObjectPool.SendAsync(stateObject);
        }

        public async Task<bool> Parse(StateObject stateObject, ClientProtocol packet)
        {
            try
            {
                switch (packet)
                {
                    case LoginRequest login:
                        return await login.Parse(stateObject);

                    case MessageRequest message:
                        return await message.Parse(stateObject);

                    default:
                        return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                packet.Dispose();
            }
        }
    }
}
