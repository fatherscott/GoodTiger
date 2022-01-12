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
        public string UID { get; set; }
        public string Room { get; set; }
        public string NickName { get; set; }
        public BufferBlock<global::Protocol.ClientProtocol> SendChan { get; set; }
        public ulong MemoryId { get; set; }

        public User Copy()
        {
            return new User
            {
                UID = this.UID,
                Room = this.Room,
                NickName = this.NickName,
                SendChan = this.SendChan,
                MemoryId = this.MemoryId
            };
        }

        public override async Task Job(ServerMemory memory)
        {
            var users = memory.Users;
            var rooms = memory.Rooms;
            var user = Copy();

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
