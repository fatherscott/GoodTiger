using Protocol;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace GoodTiger
{
    public partial class SocketManager
    {
        public async Task MainProc()
        {
            var users = new Dictionary<string, CSLogin>();
            var rooms = new Dictionary<string, Dictionary<string, CSLogin>>();
            var tasks = new List<Task>();

            while (true)
            {
                var obj = await _mainChan.ReceiveAsync();

                if (obj == null)
                {
                    break;
                }

                try
                {
                    switch (obj)
                    {

                        case CSLogin login:
                            users.Add(login.UID, login);
                            if (!rooms.ContainsKey(login.Room))
                            {
                                rooms.Add(login.Room, new Dictionary<string, CSLogin>());
                            }
                            rooms[login.Room][login.UID] = login;

                            foreach(var user in rooms[login.Room])
                            {
                                var loginResponse = new LoginResponse()
                                {
                                    UID = login.UID,
                                    NickName = login.NickName,
                                };
                                await user.Value.SendChan.SendAsync(loginResponse);
                            }
                            

                            break;

                        case CSLogout logout:
                            if (users.ContainsKey(logout.UID))
                            {
                                using var login = users[logout.UID];
                                if (rooms.ContainsKey(login.Room))
                                {
                                    rooms[login.Room].Remove(logout.UID);
                                }
                                users.Remove(logout.UID);

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
                            break;

                        case CSMessage message:
                            if (users.ContainsKey(message.UID))
                            {
                                var login = users[message.UID];
                                if (rooms.ContainsKey(login.Room))
                                {
                                    tasks.Clear();
                                    foreach (var user in rooms[login.Room])
                                    {
                                        var messageResponse = new MessageResponse()
                                        {
                                            UID = login.UID,
                                            Message = message.Message,
                                        };
                                        tasks.Add(user.Value.SendChan.SendAsync(messageResponse));
                                    }
                                    await Task.WhenAll(tasks);
                                }
                            }
                            break;
                    }
                }
                catch (Exception e)
                {
                    Logger.Instance.Error($"{e.Message}, {e.StackTrace}");
                }
            }
        }
    }
}
