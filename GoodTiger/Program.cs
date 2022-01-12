using Microsoft.Extensions.Configuration;
using NLog.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace GoodTiger
{
    class Program
    {
        public static IConfiguration Configuration { get; set; }

        static async Task Main(string[] args)
        {
            Console.WriteLine($"GoodTimger Start");

            var environment = Environment.GetEnvironmentVariable("ENVIRONMENT");

            if (environment != null)
            {
                Configuration = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                   .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
                   .Build();
            }
            else
            {
                Configuration = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                   .Build();
            }


            var nlog = Configuration.GetSection("NLog");
            NLog.LogManager.Configuration = new NLogLoggingConfiguration(nlog);

            var port = Configuration.GetValue<int>("Port");
            var poolSize = Configuration.GetValue<int>("PoolSize");

            SocketManager manager = new SocketManager();
            manager.Initialization(port, poolSize);
            await manager.StartListening();
        }
    }
}
