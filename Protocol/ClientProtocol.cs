using Microsoft.Extensions.ObjectPool;
using System;
using System.Collections.Generic;
using System.Text;

namespace Protocol
{

    public enum ProtocolType : int
    {
        None = 0,
        LoginRequest = 1,
        LoginResponse,
        LogoutRequest,
        LogoutResponse,
        MessageRequest,
        MessageResponse,
    };

    public abstract class ClientProtocol : Base
    {
        public abstract ProtocolType Type { get; }
        public static ClientProtocol Get()
        {
            return null;
        }

        public const int PoolSize = 10;
    }

}
