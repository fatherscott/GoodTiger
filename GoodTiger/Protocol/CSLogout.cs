using Microsoft.Extensions.ObjectPool;
using Protocol;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Threading;
using GoodTiger.Protocol;
using GoodTiger.Model;

namespace SocketServer
{
    public class CSLogout : ServerProtocol
    {
        public AutoResetEvent MainChanExit { get; set; } = null;

        public override async Task Job(ServerMemory memory)
        {
            var users = memory.Users;
            var rooms = memory.Rooms;

            if (users.ContainsKey(UID))
            {
                var user = users[UID];

                if (!Verify(user))
                {
                    return;
                }

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
            MainChanExit?.Set();
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
