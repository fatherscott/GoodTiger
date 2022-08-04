using GoodTiger.Model;
using GoodTiger.Protocol;
using Microsoft.Extensions.ObjectPool;
using Protocol;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace GoodTiger
{
    public class CSLogout : ServerProtocol
    {
        public override async Task Job(ServerMemory memory)
        {
            var users = memory.Users;
            var rooms = memory.Rooms;

            if (users.ContainsKey(UID))
            {

                var user = users[UID];
                try
                {
                    if (rooms.ContainsKey(user.Room))
                    {
                        rooms[user.Room].Remove(UID);
                    }
                    users.Remove(UID);

                    foreach (var i in rooms[user.Room])
                    {
                        var logoutResponse = LogoutResponse.Get() as LogoutResponse;
                        logoutResponse.UID = user.UID;
                        logoutResponse.NickName = user.NickName;
                        await i.Value.SendChan.SendAsync(logoutResponse);
                    }
                }
                finally
                {
                    memory.UserPool.Return(user);
                }
            }
        }

        private static DefaultObjectPool<CSLogout> _pool;
        private static IPooledObjectPolicy<CSLogout> _policy = new DefaultPooledObjectPolicy<CSLogout>();

        static CSLogout()
        {
            _pool = new DefaultObjectPool<CSLogout>(_policy, PoolSize);
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
