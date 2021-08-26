using Newtonsoft.Json.Linq;
using Protocol;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace GoodTiger
{
    public partial class SocketManager
    {
        public async Task Send(StateObject stateObject)
        {
            while (await stateObject.SendChan.OutputAvailableAsync())
            {
                var buffer = await stateObject.SendChan.ReceiveAsync();

                if (buffer == null)
                {
                    break;
                }
                await using var Strem = new NetworkStream(stateObject.Socket);

                await Strem.WriteAsync(buffer.HeaderBuffer, 0, 4, stateObject.SendCancel.Token);
                await Strem.WriteAsync(buffer.DataBuffer, 0, buffer.Length);

                stateObject.SocketBufferPool.Return(buffer);
            }
        }

        public async Task Recv(StateObject stateObject)
        {
            //전송 채널 생성
            var sendBlock = new ActionBlock<StateObject>(Send);
            sendBlock.Post(stateObject);

            try
            {
                while (true)
                {
                    await using var strem = new NetworkStream(stateObject.Socket, false);
                    var obj = await stateObject.RecvBuffer.Read(strem, stateObject.JsonSerializer, stateObject.RecvCancel.Token);

                    if (obj != null)
                    {
                        if (!Parse(stateObject, obj))
                        {
                            throw new Exception("Parsing failed");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.Message}, {e.StackTrace}");
            }

            if (!string.IsNullOrEmpty(stateObject.UID))
            {
                CSLogout logout = new CSLogout
                {
                    UID = stateObject.UID
                };
                stateObject.MainChan.Post(logout);
            }

            stateObject.SendChan.Post(null);
            stateObject.SendCancel.Cancel();

            sendBlock.Complete();
            sendBlock.Completion.Wait();

            //await stateObject.Strem.DisposeAsync();
            stateObject.Socket.Close();
        }

        public bool Parse(StateObject stateObject, Protocol.Base packet)
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
                        stateObject.MainChan.Post(csLogin);

                        login.Dispose();
                        break;

                    case MessageRequest message:

                        var csMessage = new CSMessage();

                        if(string.IsNullOrWhiteSpace(stateObject.UID))
                        {
                            return false;
                        }

                        csMessage.UID = stateObject.UID;
                        csMessage.Message = message.Message;
                        stateObject.MainChan.Post(csMessage);

                        message.Dispose();
                        break;

                    default:
                        return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

    }
}
