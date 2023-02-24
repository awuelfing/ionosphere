using DXLib.WebAdapter;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClusterTaskRunner
{
    public class WebTaskRunner
    {
        private readonly WebAdapterClient _webAdapterClient;
        private readonly ProgramOptions _programOptions;
        private readonly ILogger<WebTaskRunner> _logger;
        public WebTaskRunner(ILogger<WebTaskRunner> logger,WebAdapterClient webAdapterClient, IOptions<ProgramOptions> programOptions)
        {
            _logger = logger;
            _webAdapterClient = webAdapterClient;
            _programOptions = programOptions.Value;
        }
        public async Task RunWebTask(string rightUri,int delay)
        {
            _logger.Log(
                LogLevel.Trace, 
                "WebTaskRunner starting for {rightUri} with delay {delay}", 
                rightUri == string.Empty?"empty string":rightUri,
                delay);
            while (true)
            {
                await Task.Delay(delay);
                _logger.Log(
                    LogLevel.Information,
                    "WebTaskRunner running for {rightUri} after delay {delay}",
                    rightUri == string.Empty ? "empty string" : rightUri,
                    delay);
                await _webAdapterClient!.DoBlindRetrieve(rightUri);
            }
        }
    }
}
