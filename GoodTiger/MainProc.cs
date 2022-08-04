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
                    if (memory.Users.ContainsKey(obj.UID))
                    {
                        var user = memory.Users[obj.UID];
                        if (user.MemoryId != obj.MemoryId)
                        {
                            //중복 로그인된 유저로 끊어지길 기다린다.
                            continue;
                        }
                    }

                    await obj.Job(memory);
                }
                catch (Exception e)
                {
                    Logger.Instance.Error($"{e.Message}, {e.StackTrace}");
                }
                finally
                {
                    obj.Return();
                }
            }
        }
    }
}
