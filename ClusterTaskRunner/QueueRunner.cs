using ClusterConnection;
using DXLib.HamQTH;
using DXLib.WebAdapter;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
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
        private readonly ILogger<QueueRunner> _logger;
        public QueueRunner(ILogger<QueueRunner> logger, IOptions<ProgramOptions> programOptions, WebAdapterClient webAdapterClient)
        {
            _logger= logger;
            _programOptions = programOptions.Value;
            _webAdapterClient = webAdapterClient;
        }

        public void ReceiveSpots(object? sender, SpotEventArgs e)
        {
            _logger.Log(LogLevel.Debug, "received {e}", e);
            if (_programOptions!.EnableQueueUploader)
            {
                _localQueue.Enqueue(e.Spottee);
                _logger.Log(LogLevel.Debug, "queued {e}", e);
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
                    _logger.Log(LogLevel.Debug, "dequeued {Callsign}", Callsign);
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
                        _logger.Log(LogLevel.Debug, "checking cache(only) for {Key}", subject.Key);
                        HamQTHResult? result = await _webAdapterClient!.GetGeoAsync(subject.Key, false);
                        if (result == null)
                        {
                            _logger.Log(LogLevel.Debug, "cache miss for {Key}, queueing", subject.Key);
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
                    _logger.Log(LogLevel.Debug, "resolving {s}",s);
                    await _webAdapterClient.GetGeoAsync(s);
                    _logger.Log(LogLevel.Debug, "resolved {s}", s);
                }
            }
        }
    }
}
