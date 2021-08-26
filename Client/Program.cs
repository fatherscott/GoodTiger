using Newtonsoft.Json;
using Protocol;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("GoodTiger Client Start!");

            JsonSerializer jsonSerializer = new JsonSerializer();

            var clients = new ActionBlock<JsonSerializer>(Client.Work, new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = 500
            });

            for(int i = 0; i < 90000; i++)
            {
                clients.Post(jsonSerializer);
            }

            clients.Complete();
            await clients.Completion;
        }
    }
}
