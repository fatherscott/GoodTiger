using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Protocol
{
    public enum ProtocolType : int
    {
        None = 0,
        LoginRequest = 1,
        LoginResponse,
        MessageRequest,
        MessageResponse,
    };

    class Type
    {
        public static async Task FillBuffer(Base obj, SocketBuffer buffer, JsonSerializer jsonSerializer)
        {
            await using var ms = new MemoryStream(buffer.DataBuffer);
            using var writer = new StreamWriter(ms);
            using var jsonWriter = new JsonTextWriter(writer);

            switch (obj)
            {
                case LoginRequest loginRequest:
                    buffer.Type = (int)ProtocolType.LoginRequest;
                    jsonSerializer.Serialize(jsonWriter, loginRequest);
                    break;

                case LoginResponse loginResponse:
                    buffer.Type = (int)ProtocolType.LoginResponse;
                    jsonSerializer.Serialize(jsonWriter, loginResponse);
                    break;

                case MessageRequest messageRequest:
                    buffer.Type = (int)ProtocolType.MessageRequest;
                    jsonSerializer.Serialize(jsonWriter, messageRequest);
                    break;

                case MessageResponse messageResponse:
                    buffer.Type = (int)ProtocolType.MessageResponse;
                    jsonSerializer.Serialize(jsonWriter, messageResponse);
                    break;
            }

            jsonWriter.Flush();
            writer.Flush();

            buffer.Length = (int)ms.Position;
            buffer.Worked = 0;
        }

        public static Base Class(ProtocolType type, JsonTextReader jsonReader, JsonSerializer jsonSerializer)
        {
            switch (type)
            {
                case ProtocolType.LoginRequest:
                    return jsonSerializer.Deserialize<LoginRequest>(jsonReader);

                case ProtocolType.LoginResponse:
                    return jsonSerializer.Deserialize<LoginResponse>(jsonReader);

                case ProtocolType.MessageRequest:
                    return jsonSerializer.Deserialize<MessageRequest>(jsonReader);

                case ProtocolType.MessageResponse:
                    return jsonSerializer.Deserialize<MessageResponse>(jsonReader);
            }

            return default;
        }
    }
}
