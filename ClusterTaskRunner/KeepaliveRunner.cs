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
    public class KeepaliveRunner
    {
        private readonly WebAdapterClient _webAdapterClient;
        private readonly ProgramOptions _programOptions;
        private readonly ILogger<KeepaliveRunner> _logger;
        public KeepaliveRunner(ILogger<KeepaliveRunner> logger,WebAdapterClient webAdapterClient, IOptions<ProgramOptions> programOptions)
        {
            _logger = logger;
            _webAdapterClient = webAdapterClient;
            _programOptions = programOptions.Value;
        }
        public async Task ProcessKeepAlive()
        {
            while (true)
            {
                await Task.Delay(_programOptions!.KeepAliveDelay);
                _logger.Log(LogLevel.Information, "Calling keepalive after {0}",_programOptions.KeepAliveDelay);
                await _webAdapterClient!.DoKeepAlive();
            }
        }
    }
}
