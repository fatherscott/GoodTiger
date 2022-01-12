using Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace GoodTiger.Parse
{
    public static class Message
    {
        public static async Task<bool> Parse(this MessageRequest request, StateObject stateObject)
        {
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
