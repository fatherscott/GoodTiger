using GoodTiger.Parse;
using Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace GoodTiger
{
    public partial class SocketManager
    {
        public static Dictionary<ProtocolType, Func<ClientProtocol, StateObject, Task<bool>>> PaserList = new();
        static SocketManager()
        {
            foreach (AssemblyName asmName in Assembly.GetExecutingAssembly().GetReferencedAssemblies())
                Assembly.Load(asmName);

            var types = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(asm => asm.GetTypes())
                .Where(t => typeof(ClientParser).IsAssignableFrom(t) && !t.IsAbstract);

            foreach (var t in types)
            {
                var method = t.GetMethod("Initialization");
                method.Invoke(null, null);
            }
        }

        public static void Initialization()
        {

        }

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

                    await using var Strem = new System.Net.Sockets.NetworkStream(stateObject.Socket);

                    var buffer = stateObject.SendSocketBufferPool.Get();
                    await buffer.Write(Strem, protocol, stateObject.SendCancel.Token);
                    stateObject.SendSocketBufferPool.Return(buffer);

                    protocol.Dispose();
                }
            }
            catch
            {
                stateObject.RecvCancel.Cancel();
                return;
            }
        }

        public async Task Recv(StateObject stateObject)
        {
            var sendTask = Task.Run(async () => await Send(stateObject));
            try
            {
                while (true)
                {
                    await using var strem = new System.Net.Sockets.NetworkStream(stateObject.Socket, false);
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
                if (PaserList.ContainsKey(packet.Type))
                {
                    return await PaserList[packet.Type](packet, stateObject);
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
            return false;
        }
    }
}
