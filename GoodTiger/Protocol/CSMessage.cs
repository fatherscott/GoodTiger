using GoodTiger.Model;
using GoodTiger.Protocol;
using Microsoft.Extensions.ObjectPool;
using Protocol;
using System;
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
            //var tasks = memory.Tasks;

            if (users.ContainsKey(UID))
            {
                var user = users[UID];
                if (!Verify(user))
                {
                    return;
                }

                var messageResponse = MessageResponse.Get() as MessageResponse;
                messageResponse.UID = user.UID;
                Array.Copy(messageResponse.Message, Message, Message.Length);
                await user.SendChan.SendAsync(messageResponse);
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
