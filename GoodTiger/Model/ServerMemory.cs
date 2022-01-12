using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodTiger.Model
{
    public class ServerMemory
    {
        public Dictionary<string, User> Users { get; set; } = new Dictionary<string, User>();
        public Dictionary<string, Dictionary<string, User>> Rooms { get; set; } = new Dictionary<string, Dictionary<string, User>>();
        public List<Task> Tasks { get; set; } = new List<Task>();
        public int ActiveRoomId { get; set; } = 0;
    }
}
