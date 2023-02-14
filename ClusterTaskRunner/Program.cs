using Microsoft.Extensions.Configuration;
using DxLib.DbCaching;
using ClusterConnection;
using System.Collections.Concurrent;
using DXLib.WebAdapter;
using DXLib.HamQTH;
using System.Diagnostics;
using Amazon.Runtime.Internal.Util;

namespace ClusterTaskRunner
{
    internal class Program
    {
        private static readonly ConcurrentQueue<string> _localQueue = new ConcurrentQueue<string>();
        private static readonly Dictionary<string,int> _localAggregatedQueue= new Dictionary<string,int>();
        private static WebAdapterClient? _webAdapterClient;
        private static ClusterClient? _clusterClient;
        private static ProgramOptions? _programOptions;
        static void ReceiveSpots(object? sender, SpotEventArgs e)
        {
            if(_programOptions!.EnableQueueUploader)
            {
                _localQueue.Enqueue(e.Spottee);
            }
            if(_programOptions!.EnableSpotUpload)
            {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                _webAdapterClient!.PostSpotAsync(e.AsSpot());
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            }
        }
        static void ReceiveDisconnect(object? sender, EventArgs e)
        {
            Console.WriteLine("Disconnected.");
        }
        static async Task PumpQueue()
        {
            DateTime lastQueueUpload = DateTime.Now;
            string? Callsign = string.Empty;
            while (true)
            {
                if (_localQueue.TryDequeue(out Callsign))
                {
                    Console.Write("Pushing");
                    if(_localAggregatedQueue.ContainsKey(Callsign))
                    {
                        _localAggregatedQueue[Callsign]++;
                    }
                    else
                    {
                        _localAggregatedQueue[Callsign] = 1;
                    }
                }

                if((DateTime.Now - lastQueueUpload).TotalMilliseconds > _programOptions!.QueueUploaderDelay)
                {
                    Console.WriteLine("Checking");
                    var subject = _localAggregatedQueue.OrderByDescending(kvp => kvp.Key).FirstOrDefault();
                    if(!subject.Equals(default(KeyValuePair<string,int>)))
                    {
                        Console.WriteLine("Got something");
                        HamQTHResult? result = await _webAdapterClient!.GetGeoAsync(subject.Key,false);
                        if (result == null)
                        {
                            Console.WriteLine("Queueing");
                            await _webAdapterClient!.Enqueue(subject.Key, subject.Value);
                        }
                        _localAggregatedQueue.Remove(subject.Key);
                    }
                    lastQueueUpload= DateTime.Now;
                }
            }
        }
        static async Task ProcessResolver()
        {
            while (true)
            {
                await Task.Delay(_programOptions!.ResolverDelay);
                string? s = await _webAdapterClient!.Dequeue();
                if(s != null)
                {
                    await _webAdapterClient.GetGeoAsync(s);
                }    
            }
        }
        static async Task ProcessKeepAlive()
        {
            while(true)
            {
                await _webAdapterClient!.DoKeepAlive();
                await Task.Delay(_programOptions!.KeepAliveDelay);
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

            WebAdapterOptions webAdapterOptions = new WebAdapterOptions();
            configurationRoot.GetSection(WebAdapterOptions.WebAdapter).Bind(webAdapterOptions);
             _webAdapterClient = new WebAdapterClient(webAdapterOptions);

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
            if (_programOptions.EnableQueueUploader)
            {
                t1 = Task.Run(() => { PumpQueue().Wait(); });
            }
            if (_programOptions.EnableQueueResolver)
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