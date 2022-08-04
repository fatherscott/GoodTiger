using GoodTiger.Model;
using GoodTiger.Protocol;
using Microsoft.Extensions.ObjectPool;
using Protocol;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace GoodTiger
{
    public class CSMessage : ServerProtocol
    {
        public char[] Message = new char[64];

        public override async Task Job(ServerMemory memory)
        {
            var users = memory.Users;
            var rooms = memory.Rooms;
            var tasks = memory.Tasks;

            if (users.ContainsKey(UID))
            {
                var login = users[UID];
                if (login.MemoryId != MemoryId)
                {
                    return;
                }

                if (rooms.ContainsKey(login.Room))
                {
                    tasks.Clear();
                    foreach (var user in rooms[login.Room])
                    {
                        var messageResponse = MessageResponse.Get() as MessageResponse;
                        messageResponse.UID = login.UID;
                        messageResponse.Message = Message;
                        tasks.Add(user.Value.SendChan.SendAsync(messageResponse));
                    }
                    await Task.WhenAll(tasks);
                }
            }
        }

        private static DefaultObjectPool<CSMessage> _pool;
        private static IPooledObjectPolicy<CSMessage> _policy = new DefaultPooledObjectPolicy<CSMessage>();

        static CSMessage()
        {
            _pool = new DefaultObjectPool<CSMessage>(_policy, PoolSize);
        }

        public new static ServerProtocol Get()
        {
            return _pool.Get();
        }
        public override void Return()
        {
            _pool.Return(this);
        }
    }
}
