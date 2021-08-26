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
                MaxDegreeOfParallelism = 100
            });

            for(int i = 0; i < 10000; i++)
            {
                clients.Post(jsonSerializer);
            }

            clients.Complete();
            clients.Completion.Wait();
        }
    }
}
