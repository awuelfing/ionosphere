using ClusterConnection;
using DXLib.RBN;
using DXLib.WebAdapter;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace ClusterTaskRunner
{
    public class SummaryReporter
    {
        private readonly ILogger<SummaryReporter> _logger;
        private readonly ProgramOptions _options;
        private readonly WebAdapterClient _client;
        private readonly BufferBlock<SpotEventArgs> _queue = new BufferBlock<SpotEventArgs>();
        private Dictionary<string, List<String>> _summary = new Dictionary<string, List<String>>();
        public (DateTime Start, DateTime End) _window;

        public SummaryReporter(ILogger<SummaryReporter> logger, IOptions<ProgramOptions> options, WebAdapterClient webAdapterClient, IHostApplicationLifetime appLifetime)
        {
            _logger = logger;
            _options = options.Value;
            _client = webAdapterClient;
            appLifetime.ApplicationStopping.Register(() => { Task t = PostSummary(); t.Wait(); });
        }
        public void ReceiveSpots(object? sender, SpotEventArgs e)
        {
            _logger.Log(LogLevel.Trace, "received {e}", e);
            _queue.Post(e);
        }

        public async Task SummaryLoop()
        {
            _window = Helper.CalcCurrentWindowUtc(_options.SummaryUploadFrequencySeconds);
            while (true)
            {
                var tokenSource = new CancellationTokenSource();
                var delayMs = Convert.ToInt32(Math.Ceiling((_window.End - DateTime.UtcNow).TotalMilliseconds));

                Task reportTask = Task.Delay(delayMs, tokenSource.Token);
                Task<bool> queueTask = _queue.OutputAvailableAsync(tokenSource.Token);

                await Task.WhenAny(queueTask, reportTask);

                if (_queue.TryReceive(out var eSpot))
                {
                    _logger.Log(LogLevel.Trace, "dequeued {spot}", eSpot);
                    if (_summary.ContainsKey(eSpot.Spottee.ToUpper()))
                    {
                        if (!_summary[eSpot.Spottee.ToUpper()].Contains(eSpot.Band))
                        {
                            _summary[eSpot.Spottee.ToUpper()].Add(eSpot.Band);
                        }
                    }
                    else
                    {
                        _summary[eSpot.Spottee.ToUpper()] = new List<String>() { eSpot.Band };
                    }
                }
                tokenSource.Cancel();
                if (DateTime.UtcNow > _window.End)
                {
                    await PostSummary();
                    _window = Helper.CalcCurrentWindowUtc(_options.SummaryUploadFrequencySeconds);
                    _logger.Log(LogLevel.Trace, "window is {start} to {end}", _window.Start, _window.End);
                }
            }
        }

        private async Task PostSummary()
        {
            await _client.PostSummary(new SummaryRecord()
            {
                RecordStartUtc = _window.Start,
                RecordEndUtc = _window.End,
                Activity = _summary,
                TaskRunnerHost = _options.ProgramHost
            });
            _logger.Log(LogLevel.Information, "Posted summary");
            _summary.Clear();
        }
    }
}
