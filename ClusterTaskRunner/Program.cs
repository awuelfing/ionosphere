using Microsoft.Extensions.Configuration;
using DxLib.DbCaching;
using ClusterConnection;
using System.Collections.Concurrent;

namespace ClusterTaskRunner
{
    internal class Program
    {
        private static readonly ConcurrentQueue<string> _queue = new ConcurrentQueue<string>();
        private static DbQueue? _dbQueue;
        static void ReceiveSpots(object? sender, SpotEventArgs e)
        {
            //Console.WriteLine($"{e.Spotter} saw {e.Spottee}");
            _queue.Enqueue(e.Spottee);
        }
        static void ReceiveDisconnect(object? sender, EventArgs e)
        {
            Console.WriteLine("Disconnected.");
        }
        static void ProcessUploads()
        {
            string? Callsign = string.Empty;
            while(true)
            {
                if(_queue.TryDequeue(out Callsign))
                {
                    // .Wait() is for rate limiting, remove if this gets clogged.
                    // Make this an option?
                    _dbQueue!.EnqueueAsync(Callsign).Wait();
                }
            }
        }
        static void Main(string[] args)
        {
            IConfigurationRoot configurationRoot = new ConfigurationBuilder()
               .SetBasePath(AppContext.BaseDirectory)
               .AddJsonFile("appsettings.json")
               .Build();
            DbCacheOptions dbCacheOptions = new DbCacheOptions();
            configurationRoot.GetSection(DbCacheOptions.DbCache).Bind(dbCacheOptions);

            _dbQueue = new DbQueue(dbCacheOptions);

            ClusterClientOptions clusterClientOptions = new ClusterClientOptions();
            configurationRoot.GetSection(ClusterClientOptions.ClusterClient).Bind(clusterClientOptions);

            ClusterClient clusterClient = new ClusterClient(clusterClientOptions.Host, clusterClientOptions.Port,clusterClientOptions.Callsign);
            if (!clusterClient.Connect())
            {
                Console.WriteLine("Failed to connect.");
                return;
            }
            clusterClient.SpotReceived += ReceiveSpots;
            clusterClient.Disconnected += ReceiveDisconnect;

            Task t = Task.Run(() => { ProcessUploads(); });

            clusterClient.ProcessSpots();

        }
    }
}