using GoodTiger.Model;
using GoodTiger.Protocol;
using Protocol;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace GoodTiger
{
    public class CSMessage : ServerProtocol
    {
        public string Message { get; set; }

        public override async Task Job(ServerMemory memory)
        {
            var users = memory.Users;
            var rooms = memory.Rooms;
            var tasks = memory.Tasks;

            if (users.ContainsKey(UID))
            {
                var login = users[UID];
                if (login.MemoryId != MemoryId)
                {
                    return;
                }

                if (rooms.ContainsKey(login.Room))
                {
                    tasks.Clear();
                    foreach (var user in rooms[login.Room])
                    {
                        var messageResponse = new MessageResponse()
                        {
                            UID = login.UID,
                            Message = Message,
                        };
                        tasks.Add(user.Value.SendChan.SendAsync(messageResponse));
                    }
                    await Task.WhenAll(tasks);
                }
            }
        }
    }
}
