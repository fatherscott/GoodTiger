using Microsoft.Extensions.ObjectPool;
using System;

namespace Protocol
{
    public class LoginRequest : ClientProtocol
    {
        public override ProtocolType Type => ProtocolType.LoginRequest;
        public string UID { get; set; }
        public string Room { get; set; }
        public string NickName { get; set; }

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
