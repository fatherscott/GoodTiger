using GoodTiger.Model;
using GoodTiger.Protocol;
using Protocol;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace GoodTiger
{
    public class CSLogout : ServerProtocol
    {

        public override async Task Job(ServerMemory memory)
        {
            var users = memory.Users;
            var rooms = memory.Rooms;

            if (users.ContainsKey(UID))
            {

                var user = users[UID];
                try
                {
                    if (rooms.ContainsKey(user.Room))
                    {
                        rooms[user.Room].Remove(UID);
                    }
                    users.Remove(UID);

                    foreach (var i in rooms[user.Room])
                    {
                        var logoutResponse = new LogoutResponse()
                        {
                            UID = user.UID,
                            NickName = user.NickName,
                        };
                        await i.Value.SendChan.SendAsync(logoutResponse);
                    }
                }
                finally
                {
                    memory.UserPool.Return(user);
                }
            }
        }
    }
}
