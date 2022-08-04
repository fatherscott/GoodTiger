using Microsoft.Extensions.ObjectPool;
using System;

namespace Protocol
{
    public class LogoutRequest : ClientProtocol
    {
        public override ProtocolType Type => ProtocolType.LogoutRequest;

        private static DefaultObjectPool<LogoutRequest> _pool;
        private static IPooledObjectPolicy<LogoutRequest> _policy = new DefaultPooledObjectPolicy<LogoutRequest>();

        static LogoutRequest()
        {
            _pool = new DefaultObjectPool<LogoutRequest>(_policy, PoolSize);
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
