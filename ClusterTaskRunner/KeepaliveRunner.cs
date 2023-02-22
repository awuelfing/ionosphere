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
        public KeepaliveRunner(WebAdapterClient webAdapterClient, IOptions<ProgramOptions> programOptions)
        {
            _webAdapterClient = webAdapterClient;
            _programOptions = programOptions.Value;
        }
        public async Task ProcessKeepAlive()
        {
            Log.Verbose("Keepalive running");
            while (true)
            {
                Log.Information("Calling keepalive");
                await _webAdapterClient!.DoKeepAlive();
                await Task.Delay(_programOptions!.KeepAliveDelay);
            }
        }
    }
}
