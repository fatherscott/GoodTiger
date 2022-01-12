using GoodTiger.Model;
using GoodTiger.Protocol;
using Protocol;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace GoodTiger
{
    public class CSLogout : ServerProtocol
    {
        public string UID { get; set; }

        public override async Task Job(ServerMemory memory)
        {
            var users = memory.Users;
            var rooms = memory.Rooms;

            if (users.ContainsKey(UID))
            {
                using var login = users[UID];
                if (rooms.ContainsKey(login.Room))
                {
                    rooms[login.Room].Remove(UID);
                }
                users.Remove(UID);

                foreach (var user in rooms[login.Room])
                {
                    var logoutResponse = new LogoutResponse()
                    {
                        UID = login.UID,
                        NickName = login.NickName,
                    };
                    await user.Value.SendChan.SendAsync(logoutResponse);
                }
            }
        }
    }
}
