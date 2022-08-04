using Microsoft.Extensions.ObjectPool;
using System;
using System.Collections.Generic;
using System.Text;

namespace Protocol
{
    public class LoginResponse : ClientProtocol
    {
        public override ProtocolType Type => ProtocolType.LoginResponse;
        public long UID { get; set; }
        public char[] NickName = new char[32];

        private static DefaultObjectPool<LoginResponse> _pool;
        private static IPooledObjectPolicy<LoginResponse> _policy = new DefaultPooledObjectPolicy<LoginResponse>();

        static LoginResponse()
        {
            _pool = new DefaultObjectPool<LoginResponse>(_policy, PoolSize);
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
