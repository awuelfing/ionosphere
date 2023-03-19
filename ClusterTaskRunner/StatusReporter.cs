using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClusterTaskRunner
{
    public class StatusReporter
    {
        private readonly ILogger<StatusReporter> _logger;
        private readonly ProgramOptions _options;
        public int SpotsUploaded { get; set; } = 0;
        public StatusReporter(ILogger<StatusReporter> logger, IOptions<ProgramOptions> options)
        {
            _logger = logger;
            _options = options.Value;
        }
        public void ReportSpotUpload(object? sender, EventArgs eventArgs)
        {
            SpotsUploaded++;
        }
        public async Task StatusLoop()
        {
            while (true)
            {
                await Task.Delay(_options.StatusReportDelay);
                _logger.Log(LogLevel.Information, "Spots uploaded: {SpotsUploaded}", this.SpotsUploaded);
                int availableThreads, completionThreads, maxThreads;
                ThreadPool.GetAvailableThreads(out availableThreads, out completionThreads);
                ThreadPool.GetMaxThreads(out maxThreads, out completionThreads);
                _logger.Log(
                    LogLevel.Trace,
                    "{availableThreads}/{maxThreads} ThreadPool availability, PendingWorkItemCount {PendingWorkItemCount}",
                    availableThreads,
                    maxThreads,
                    ThreadPool.PendingWorkItemCount);
            }
        }
    }
}
