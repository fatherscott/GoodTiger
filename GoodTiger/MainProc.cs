using GoodTiger.Model;
using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace GoodTiger
{
    public partial class SocketManager
    {
        public async Task MainProc()
        {
            ServerMemory memory = new();

            while (true)
            {
                var obj = await _mainChan.ReceiveAsync();

                if (obj == null)
                {
                    break;
                }

                try
                {
                    await obj.Job(memory);
                }
                catch (Exception e)
                {
                    Logger.Instance.Error($"{e.Message}, {e.StackTrace}");
                }
                finally
                {
                    obj.Dispose();
                }
            }
        }
    }
}
