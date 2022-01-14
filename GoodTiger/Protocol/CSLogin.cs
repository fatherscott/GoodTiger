using GoodTiger.Model;
using GoodTiger.Protocol;
using Protocol;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace GoodTiger
{
    public class CSLogin : ServerProtocol
    {

        public string Room { get; set; }
        public string NickName { get; set; }
        public BufferBlock<ClientProtocol> SendChan { get; set; }

        public User Copy(ServerMemory memory)
        {
            var user = memory.UserPool.Get();
            user.UID = UID;
            user.Room = Room;
            user.NickName = NickName;
            user.SendChan = SendChan;
            user.MemoryId = MemoryId;
            return user;
        }

        public override async Task Job(ServerMemory memory)
        {
            var users = memory.Users;
            var rooms = memory.Rooms;
            var user = Copy(memory);

            users.Add(UID, user);
            if (!rooms.ContainsKey(Room))
            {
                rooms.Add(Room, new Dictionary<string, User>());
            }
            rooms[Room][UID] = user;

            foreach (var i in rooms[Room])
            {
                var loginResponse = new LoginResponse()
                {
                    UID = UID,
                    NickName = NickName,
                };
                await i.Value.SendChan.SendAsync(loginResponse);
            }
        }
    }
}
