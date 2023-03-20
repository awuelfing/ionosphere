using DXLib.ClusterList;
using DXLib.WebAdapter;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClusterTaskRunner
{
    public class ClusterReporter
    {
        private readonly ILogger<ClusterReporter> _logger;
        private readonly WebAdapterClient _webAdapterClient;
        private readonly ProgramOptions _programOptions;
        public ClusterReporter(ILogger<ClusterReporter> logger, WebAdapterClient webAdapterClient, IOptions<ProgramOptions> programOptions)
        {
            _logger = logger;
            _webAdapterClient = webAdapterClient;
            _programOptions = programOptions.Value;
        }
        public async Task ReportCluster()
        {
            _logger.Log(LogLevel.Information, "Retrieving cluster information");
            var report = await ClusterListFetcher.FetchClusterResultsAsync();
            _logger.Log(LogLevel.Information, "Posting cluster information");
            await _webAdapterClient.PostClusterAsync(new ClusterRecord()
            {
                RetrievalDate = DateTime.UtcNow,
                ClusterNodes = report.AsClusterNodes()
            });
            _logger.Log(LogLevel.Information, "Cluster post successful");
        }
    }
}
