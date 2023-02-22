using ClusterConnection;
using DXLib.HamQTH;
using DXLib.WebAdapter;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClusterTaskRunner
{
    public class QueueRunner
    {
        private readonly ProgramOptions _programOptions;
        private readonly WebAdapterClient _webAdapterClient;
        private readonly ConcurrentQueue<string> _localQueue = new ConcurrentQueue<string>();
        private readonly Dictionary<string, int> _localAggregatedQueue = new Dictionary<string, int>();
        public QueueRunner(IOptions<ProgramOptions> programOptions, IOptions<WebAdapterClient> webAdapterClient)
        {
            _programOptions = programOptions.Value;
            _webAdapterClient = webAdapterClient.Value;
        }

        public void ReceiveSpots(object? sender, SpotEventArgs e)
        {
            if (_programOptions!.EnableQueueUploader)
            {
                _localQueue.Enqueue(e.Spottee);
            }
        }
        public async Task PumpQueue()
        {
            DateTime lastQueueUpload = DateTime.Now;
            string? Callsign = string.Empty;
            while (true)
            {
                if (_localQueue.TryDequeue(out Callsign))
                {
                    if (_localAggregatedQueue.ContainsKey(Callsign))
                    {
                        _localAggregatedQueue[Callsign]++;
                    }
                    else
                    {
                        _localAggregatedQueue[Callsign] = 1;
                    }
                }

                if ((DateTime.Now - lastQueueUpload).TotalMilliseconds > _programOptions!.QueueUploaderDelay)
                {
                    var subject = _localAggregatedQueue.OrderByDescending(kvp => kvp.Key).FirstOrDefault();
                    if (!subject.Equals(default(KeyValuePair<string, int>)))
                    {
                        HamQTHResult? result = await _webAdapterClient!.GetGeoAsync(subject.Key, false);
                        if (result == null)
                        {
                            await _webAdapterClient!.Enqueue(subject.Key, subject.Value);
                        }
                        _localAggregatedQueue.Remove(subject.Key);
                    }
                    lastQueueUpload = DateTime.Now;
                }
            }
        }
        public async Task ProcessResolver()
        {
            while (true)
            {
                await Task.Delay(_programOptions!.ResolverDelay);
                string? s = await _webAdapterClient!.Dequeue();
                if (s != null)
                {
                    await _webAdapterClient.GetGeoAsync(s);
                }
            }
        }
    }
}
