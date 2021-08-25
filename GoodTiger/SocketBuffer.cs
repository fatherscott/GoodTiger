using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GoodTiger
{
    public class SocketBuffer
    {
        // Size of receive buffer.  
        public const int DataBufferSize = 1024 * 1024;

        // Receive buffer.  
        public byte[] DataBuffer = new byte[DataBufferSize];

        // Size of receive buffer.  
        public const int HeaderBufferSize = 4;

        // Receive buffer.  
        public byte[] HeaderBuffer = new byte[HeaderBufferSize];

        public int Length
        {
            get
            {
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(HeaderBuffer);
                return BitConverter.ToInt32(HeaderBuffer);
            }
            set
            {
                BitConverter.TryWriteBytes(HeaderBuffer, value);
            }
        }

        public int Worked { get; set; } = 0;

        public async Task RecvHead(NetworkStream strem, CancellationToken cancellationToken)
        {
            await strem.ReadAsync(HeaderBuffer, 0, 4, cancellationToken);
        }

        public async Task RecvData(NetworkStream strem, CancellationToken cancellationToken)
        {
            Worked = 0;
            for (; Worked < Length;)
            {
                var recved = await strem.ReadAsync(DataBuffer, Worked, Length - Worked, cancellationToken);
                if (recved == 0)
                {
                    throw new SocketException(1006);
                }
                else
                {
                    Worked += recved;
                }
            }
        }
    }
}
