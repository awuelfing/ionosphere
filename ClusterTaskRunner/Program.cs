using Microsoft.Extensions.Configuration;
using DxLib.DbCaching;
using ClusterConnection;
using System.Collections.Concurrent;
using DXLib.WebAdapter;
using DXLib.HamQTH;
using System.Diagnostics;

namespace ClusterTaskRunner
{
    internal class Program
    {
        private static readonly ConcurrentQueue<string> _queue = new ConcurrentQueue<string>();
        private static DbQueue? _dbQueue;
        private static DbCache? _dbCache;
        private static WebAdapterClient? _webAdapterClient;
        private static ClusterClient? _clusterClient;
        private static ProgramOptions? _programOptions;
        static void ReceiveSpots(object? sender, SpotEventArgs e)
        {
            _queue.Enqueue(e.Spottee);
        }
        static void ReceiveDisconnect(object? sender, EventArgs e)
        {
            Console.WriteLine("Disconnected.");
        }
        static async Task ProcessUploads()
        {
            string? Callsign = string.Empty;
            while (true)
            {
                if (_queue.TryDequeue(out Callsign))
                {
                    HamQTHResult? result = await _dbCache!.GetGeoAsync(Callsign);
                    if (result == null)
                    {
                        // .Wait() is for rate limiting, remove if this gets clogged.
                        // Make this an option?
                        await _dbQueue!.EnqueueAsync(Callsign);
                    }
                }
            }
        }
        static async Task ProcessResolver()
        {
            while (true)
            {
                await Task.Delay(_programOptions!.ResolverDelay*1000);
                DbQueueRecord? record = await _dbQueue!.DequeueAsync(false);
                if (record != null)
                {
                    Debug.WriteLine($"Attempting to resolve {record.Callsign}");
                    HamQTHResult? result = await _webAdapterClient!.GetGeoAsync(record.Callsign);

                    if (result!.SearchResult != null)
                    {
                        Debug.WriteLine($"Resolved {result.SearchResult.callsign}");
                    }
                }
            }
        }
        static async Task ProcessKeepAlive()
        {
            while(true)
            {
                await _webAdapterClient!.DoKeepAlive();
                await Task.Delay(_programOptions!.KeepAliveDelay *1000);
            }
        }
        static void Main(string[] args)
        {
            IConfigurationRoot configurationRoot = new ConfigurationBuilder()
               .SetBasePath(AppContext.BaseDirectory)
               .AddJsonFile("appsettings.json")
               .Build();

            _programOptions = new ProgramOptions();
            configurationRoot.GetSection(ProgramOptions.ProgramOptionName).Bind(_programOptions);

            if (_programOptions.EnableUploader || _programOptions.EnableResolver)
            {
                DbCacheOptions dbCacheOptions = new DbCacheOptions();
                configurationRoot.GetSection(DbCacheOptions.DbCache).Bind(dbCacheOptions);
                _dbQueue = new DbQueue(dbCacheOptions);
                _dbCache = new DbCache(dbCacheOptions);
            }

            if (_programOptions.EnableResolver || _programOptions.EnableKeepAlive)
            {
                WebAdapterOptions webAdapterOptions = new WebAdapterOptions();
                configurationRoot.GetSection(WebAdapterOptions.WebAdapter).Bind(webAdapterOptions);
                _webAdapterClient = new WebAdapterClient(webAdapterOptions);
            }

            if (_programOptions.EnableClusterConnection)
            {
                ClusterClientOptions clusterClientOptions = new ClusterClientOptions();
                configurationRoot.GetSection(ClusterClientOptions.ClusterClient).Bind(clusterClientOptions);
                _clusterClient = new ClusterClient(clusterClientOptions.Host, clusterClientOptions.Port, clusterClientOptions.Callsign);

                if (!_clusterClient.Connect())
                {
                    Console.WriteLine("Failed to connect.");
                    return;
                }
                _clusterClient.SpotReceived += ReceiveSpots;
                _clusterClient.Disconnected += ReceiveDisconnect;
            }

            Task? t1 = null, t2 = null, t3 = null,t4 = null;
            if (_programOptions.EnableUploader)
            {
                t1 = Task.Run(() => { ProcessUploads().Wait(); });
            }
            if (_programOptions.EnableResolver)
            {
                t2 = Task.Run(() => { ProcessResolver().Wait(); });
            }
            if (_programOptions.EnableClusterConnection)
            {
                t3 = Task.Run(() => { _clusterClient!.ProcessSpots(); });
            }
            if(_programOptions.EnableKeepAlive)
            {
                t4 = Task.Run(() => { ProcessKeepAlive().Wait(); });
            }

            t1?.Wait();
            t2?.Wait();
            t3?.Wait();
            t4?.Wait();
        }
    }
}