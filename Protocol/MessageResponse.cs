using Microsoft.Extensions.ObjectPool;
using System;
using System.Collections.Generic;
using System.Text;

namespace Protocol
{
    public class MessageResponse : ClientProtocol
    {
        public override ProtocolType Type => ProtocolType.MessageResponse;
        public string UID { get; set; }
        public string Message { get; set; }

        private static DefaultObjectPool<MessageResponse> _pool;
        private static IPooledObjectPolicy<MessageResponse> _policy = new DefaultPooledObjectPolicy<MessageResponse>();

        static MessageResponse()
        {
            _pool = new DefaultObjectPool<MessageResponse>(_policy, PoolSize);
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
