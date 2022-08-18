using GoodTiger;
using Newtonsoft.Json;
using Protocol;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Client
{
    class Program
    {
        //static BufferBlock<Socket> _serverSocektChan { get; set; } = new BufferBlock<Socket>();
        static async Task Main(string[] args)
        {
            List<Task> tasks = new List<Task>();
            for (int i = 1; i < 9999999; i += 1000)
            {
                tasks.Clear();
                for (int ii = 0; ii < 1000; ii++)
                {
                    tasks.Add(Client.TestClient(i + ii));
                }
                await Task.WhenAll(tasks.ToArray());
            }

            //SocketManager manager = new SocketManager();
            //manager.Initialization(11000, 1000);
            //var server = Task.Run(async () =>
            //    await manager.StartListening(_serverSocektChan)
            //);

            //Socket listener = null;

            //try
            //{
            //    listener = await _serverSocektChan.ReceiveAsync(TimeSpan.FromSeconds(5));
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e);
            //}

            //await Task.Delay(1000);

            ////for (int i = 0; i < 1000; i++)
            //{
            //    int i = 0;
            //    var task1 = Client.TestClient(i);
            //    var task2 = Client.TestClient(i++);
            //    var task3 = Client.TestClient(i++);
            //    var task4 = Client.TestClient(i++);
            //    var task5 = Client.TestClient(i++);

            //    await Task.WhenAll(task1, task2, task3, task4, task5);
            //    //await Task.WhenAll(task1);
            //}

            //listener.Close();
            //await Task.WhenAll(server);
        }
    }
}
