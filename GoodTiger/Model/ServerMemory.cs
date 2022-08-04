using Microsoft.Extensions.ObjectPool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodTiger.Model
{
    public class ServerMemory
    {
        public Dictionary<long, User> Users { get; set; } = new Dictionary<long, User>();
        public Dictionary<long, Dictionary<long, User>> Rooms { get; set; } = new Dictionary<long, Dictionary<long, User>>();
        public List<Task> Tasks { get; set; } = new List<Task>();
        public int ActiveRoomId { get; set; } = 0;

        public ObjectPool<User> UserPool = ObjectPool.Create<User>();
    }
}
