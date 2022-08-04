using Microsoft.Extensions.ObjectPool;
using System;
using System.Collections.Generic;
using System.Text;

namespace Protocol
{
    public class LogoutResponse : ClientProtocol
    {
        public override ProtocolType Type => ProtocolType.LogoutResponse;
        public string UID { get; set; }
        public string NickName { get; set; }

        private static DefaultObjectPool<LogoutResponse> _pool;
        private static IPooledObjectPolicy<LogoutResponse> _policy = new DefaultPooledObjectPolicy<LogoutResponse>();

        static LogoutResponse()
        {
            _pool = new DefaultObjectPool<LogoutResponse>(_policy, PoolSize);
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
