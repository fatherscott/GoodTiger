using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace GoodTiger
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine($"GoodTimger Start");

            SocketManager manager = new SocketManager();
            manager.Initialization(30000);
            await manager.StartListening();
        }
    }
}
