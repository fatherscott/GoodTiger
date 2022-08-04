using Microsoft.Extensions.ObjectPool;
using System;
using System.Collections.Generic;
using System.Text;

namespace Protocol
{
    public class MessageRequest : ClientProtocol
    {
        public override ProtocolType Type => ProtocolType.MessageRequest;
        public char[] Message = new char[64];

        private static DefaultObjectPool<MessageRequest> _pool;
        private static IPooledObjectPolicy<MessageRequest> _policy = new DefaultPooledObjectPolicy<MessageRequest>();

        static MessageRequest()
        {
            _pool = new DefaultObjectPool<MessageRequest>(_policy, PoolSize);
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
