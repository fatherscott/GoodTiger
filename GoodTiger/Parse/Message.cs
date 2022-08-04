using Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace GoodTiger.Parse
{
    public class Message : ClientParser
    {
        public static new void Initialization()
        {
            SocketManager.PaserList[ProtocolType.MessageRequest] = Parse;
        }

        public static async Task<bool> Parse(ClientProtocol packet, StateObject stateObject)
        {
            var request = packet as MessageRequest;
            if (stateObject.UID == 0)
            {
                return false;
            }
            var csMessage = CSMessage.Get() as CSMessage;
            csMessage.UID = stateObject.UID;
            csMessage.MemoryId = stateObject.MemoryId;

            Array.Copy(request.Message, csMessage.Message, request.Message.Length);

            await stateObject.MainChan.SendAsync(csMessage);

            //Logger.Instance.Trace($"message {stateObject.UID}, {request.Message}");
            return true;
        }
    }
}
