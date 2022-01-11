using Newtonsoft.Json.Linq;
using Protocol;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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

                    var buffer = stateObject.SocketBufferPool.Get();
                    await buffer.Write(Strem, protocol, _jsonSerializer, stateObject.SendCancel.Token);
                    stateObject.SocketBufferPool.Return(buffer);

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
                    var obj = await stateObject.RecvBuffer.Read(strem, stateObject.JsonSerializer, stateObject.RecvCancel.Token);

                    if (obj != null)
                    {
                        if (!await Parse(stateObject, obj))
                        {
                            throw new Exception("Parsing failed");
                        }
                    }
                }
            }
            catch (Exception)
            {
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

            await stateObject.Recycling.SendAsync(stateObject);
        }

        public async Task<bool> Parse(StateObject stateObject, Base packet)
        {
            try
            {
                switch (packet)
                {
                    case LoginRequest login:

                        stateObject.UID = login.UID;

                        var csLogin = new CSLogin();
                        csLogin.UID = login.UID;
                        csLogin.Room = login.Room;
                        csLogin.NickName = login.NickName;
                        csLogin.SendChan = stateObject.SendChan;
                        await stateObject.MainChan.SendAsync(csLogin);
                        break;

                    case MessageRequest message:

                        var csMessage = new CSMessage();

                        if (string.IsNullOrWhiteSpace(stateObject.UID))
                        {
                            return false;
                        }

                        csMessage.UID = stateObject.UID;
                        csMessage.Message = message.Message;
                        await stateObject.MainChan.SendAsync(csMessage);

                        Logger.Instance.Trace($"message {stateObject.UID}, { message.Message}");
                        break;

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
            return true;
        }

    }
}
