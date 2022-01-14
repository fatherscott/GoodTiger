using Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace GoodTiger.Parse
{
    public class Message
    {
        public static void Initialization()
        {
            lock (SocketManager.PaserLock)
            {
                SocketManager.PaserList[ProtocolType.MessageRequest] = Parse;
            }
        }

        public static async Task<bool> Parse(ClientProtocol packet, StateObject stateObject)
        {
            var request = packet as MessageRequest;
            if (string.IsNullOrWhiteSpace(stateObject.UID))
            {
                return false;
            }
            var csMessage = new CSMessage();
            csMessage.UID = stateObject.UID;
            csMessage.MemoryId = stateObject.MemoryId;
            csMessage.Message = request.Message;
            await stateObject.MainChan.SendAsync(csMessage);

            Logger.Instance.Trace($"message {stateObject.UID}, { request.Message}");
            return true;
        }
    }
}
