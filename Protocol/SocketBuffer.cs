using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Protocol
{
    public class SocketBuffer
    {
        // Size of receive buffer.  
        public const int DataBufferSize = 1024 * 1024;

        // Receive buffer.  
        public byte[] DataBuffer = new byte[DataBufferSize];

        // Size of receive buffer.  
        public const int TypeBufferSize = 4;

        // Receive buffer.  
        public byte[] TypeBuffer = new byte[TypeBufferSize];

        // Size of receive buffer.  
        public const int HeaderBufferSize = 4;

        // Receive buffer.  
        public byte[] HeaderBuffer = new byte[HeaderBufferSize];

        public int Length
        {
            get
            {

                return BitConverter.ToInt32(HeaderBuffer);
            }
            set
            {


                BitConverter.TryWriteBytes(HeaderBuffer, value);

            }
        }

        public int Type
        {
            get
            {
                return BitConverter.ToInt32(TypeBuffer);
            }
            set
            {
                BitConverter.TryWriteBytes(TypeBuffer, value);
            }
        }

        public int Worked { get; set; } = 0;

        protected async Task ReadType(NetworkStream strem, CancellationToken cancellationToken = default)
        {
            int recved = 0;

            while (recved < 4)
            {
                var now = await strem.ReadAsync(TypeBuffer, recved, 4 - recved, cancellationToken);
                if (now == 0)
                {
                    throw new SocketException(1006);
                }
                recved += now;
            }

            if (!BitConverter.IsLittleEndian)
                Array.Reverse(TypeBuffer);
        }

        protected async Task ReadHead(NetworkStream strem, CancellationToken cancellationToken)
        {
            int recved = 0;

            while (recved < 4)
            {
                var now = await strem.ReadAsync(HeaderBuffer, recved, 4 - recved, cancellationToken);
                if (now == 0)
                {
                    throw new SocketException(1006);
                }
                recved += now;
            }

            if (!BitConverter.IsLittleEndian)
                Array.Reverse(HeaderBuffer);
        }

        protected async Task<ClientProtocol> ReadData(NetworkStream strem, CancellationToken cancellationToken = default)
        {
            Worked = 0;
            for (; Worked < Length;)
            {
                var now = await strem.ReadAsync(DataBuffer, Worked, Length - Worked, cancellationToken);
                if (now == 0)
                {
                    throw new SocketException(1006);
                }
                Worked += now;
            }

            await using var ms = new MemoryStream(DataBuffer, 0, Worked);
            using var reader = new StreamReader(ms);
            return ProtocolConverter.StreamToClass((ProtocolType)Type, reader);
        }

        public async Task<ClientProtocol> Read(NetworkStream strem, CancellationToken cancellationToken = default)
        {
            await ReadHead(strem, cancellationToken);
            await ReadType(strem, cancellationToken);
            return await ReadData(strem, cancellationToken);
        }

        protected async Task WriteType(NetworkStream strem, CancellationToken cancellationToken = default)
        {
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(TypeBuffer);

            await strem.WriteAsync(TypeBuffer, 0, 4, cancellationToken);
        }

        protected async Task WriteHead(NetworkStream strem, CancellationToken cancellationToken)
        {
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(HeaderBuffer);

            await strem.WriteAsync(HeaderBuffer, 0, 4, cancellationToken);
        }

        protected async Task WriteData(NetworkStream strem, CancellationToken cancellationToken = default)
        {
            await strem.WriteAsync(DataBuffer, Worked, Length - Worked, cancellationToken);
        }

        public async Task Write(NetworkStream strem, ClientProtocol protocol, CancellationToken cancellationToken = default)
        {
            await ProtocolConverter.FillBuffer(protocol, this);

            await WriteHead(strem, cancellationToken);
            await WriteType(strem, cancellationToken);
            await WriteData(strem, cancellationToken);
        }
    }
}
