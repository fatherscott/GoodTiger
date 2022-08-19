using GoodTiger.Parse;
using Protocol;
using SocketServer;
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
                while (true)
                {
                    var protocol = await stateObject.SendChan.ReceiveAsync();

                    if (protocol == null)
                    {
                        return;
                    }

                    await using var Strem = new System.Net.Sockets.NetworkStream(stateObject.Socket);

                    var buffer = stateObject.SendSocketBufferPool.Get();
                    await buffer.Write(Strem, protocol);
                    stateObject.SendSocketBufferPool.Return(buffer);

                    protocol.Return();
                }
            }
            catch
            {
                return;
            }
        }

        public async Task Recv(StateObject stateObject)
        {
            Task sendTask = null; 
            try
            {
                sendTask = Task.Run(async () => await Send(stateObject));

                while (true)
                {
                    await using var strem = new System.Net.Sockets.NetworkStream(stateObject.Socket, false);
                    using var obj = await stateObject.RecvBuffer.Read(strem, stateObject.RecvCancel.Token);

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

            if (stateObject.UID != 0)
            {
                CSLogout logout = CSLogout.Get() as CSLogout;
                logout.SetState(stateObject);
                logout.MainChanExit = stateObject.MainChanExit;
                await stateObject.MainChan.SendAsync(logout);

                stateObject.MainChanExit.WaitOne();
            }

            await stateObject.SendChan.SendAsync(null);
            stateObject.SendChan.Complete();
            await Task.WhenAll(sendTask);

            Logger.Instance.Trace($"exit {stateObject.UID}");

            stateObject.Socket.Close();

            await stateObject.Clear();
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
                packet.Return();
            }
            return false;
        }
    }
}
