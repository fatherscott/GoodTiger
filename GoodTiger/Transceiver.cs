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

                await using var strem = stateObject.Client.GetStream();
                await strem.WriteAsync(buffer.HeaderBuffer, 0, 4, stateObject.SendCancel.Token);
                await strem.WriteAsync(buffer.DataBuffer, 0, buffer.Length);

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
                    await using var strem = stateObject.Client.GetStream();
                    await stateObject.RecvBuffer.RecvHead(strem, stateObject.RecvCancel.Token);
                    await stateObject.RecvBuffer.RecvData(strem, stateObject.RecvCancel.Token);

                    //메인 프로세스 호출

                    var sendBuffer = stateObject.SocketBufferPool.Get();
                    sendBuffer.Length = stateObject.RecvBuffer.Length;
                    Buffer.BlockCopy(stateObject.RecvBuffer.DataBuffer, 0, sendBuffer.DataBuffer, 0, stateObject.RecvBuffer.Length);

                    stateObject.SendChan.Post(sendBuffer);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.Message}, {e.StackTrace}");
            }

            stateObject.SendChan.Post(null);
            stateObject.SendCancel.Cancel();

            sendBlock.Complete();
            sendBlock.Completion.Wait();

            stateObject.Client.Close();
        }

        public async Task Parse(StateObject stateObject)
        {

        }

    }
}
