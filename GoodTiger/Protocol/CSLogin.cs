using GoodTiger.Model;
using GoodTiger.Protocol;
using Microsoft.Extensions.ObjectPool;
using Protocol;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace GoodTiger
{
    public class CSLogin : ServerProtocol
    {

        public long Room { get; set; }
        public char[] NickName = new char[32];
        public BufferBlock<ClientProtocol> SendChan { get; set; }

        public User Copy(ServerMemory memory)
        {
            var user = memory.UserPool.Get();
            user.UID = UID;
            user.Room = Room;
            user.NickName = NickName;
            user.SendChan = SendChan;
            user.MemoryId = MemoryId;
            return user;
        }

        public override async Task Job(ServerMemory memory)
        {
            var users = memory.Users;
            var rooms = memory.Rooms;
            var user = Copy(memory);

            users.Add(UID, user);
            if (!rooms.ContainsKey(Room))
            {
                rooms.Add(Room, new Dictionary<long, User>());
            }
            rooms[Room][UID] = user;

            foreach (var i in rooms[Room])
            {
                var loginResponse = LoginResponse.Get() as LoginResponse;
                loginResponse.UID = UID;
                loginResponse.NickName = NickName;
                await i.Value.SendChan.SendAsync(loginResponse);
            }
        }

        private static DefaultObjectPool<CSLogin> _pool;
        private static IPooledObjectPolicy<CSLogin> _policy = new DefaultPooledObjectPolicy<CSLogin>();

        static CSLogin()
        {
            _pool = new DefaultObjectPool<CSLogin>(_policy, PoolSize);
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
