using Microsoft.Extensions.ObjectPool;
using System;

namespace Protocol
{
    public class LoginRequest : ClientProtocol
    {
        public override ProtocolType Type => ProtocolType.LoginRequest;
        public long UID { get; set; }
        public long Room { get; set; }
        public  char[] NickName = new char[32];

        private static DefaultObjectPool<LoginRequest> _pool;
        private static IPooledObjectPolicy<LoginRequest> _policy = new DefaultPooledObjectPolicy<LoginRequest>();

        static LoginRequest()
        {
            _pool = new DefaultObjectPool<LoginRequest>(_policy, PoolSize);
        }

        public new static ClientProtocol Get()
        {
            return _pool.Get();
        }
        public override void Return()
        {
            _pool.Return(this);
        }
    }
}
