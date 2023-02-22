using DXLib.WebAdapter;
using Microsoft.Extensions.Options;
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
        public KeepaliveRunner(IOptions<WebAdapterClient> webAdapterClient, IOptions<ProgramOptions> programOptions)
        {
            _webAdapterClient = webAdapterClient.Value;
            _programOptions = programOptions.Value;
        }
        public async Task ProcessKeepAlive()
        {
            while (true)
            {
                await _webAdapterClient!.DoKeepAlive();
                await Task.Delay(_programOptions!.KeepAliveDelay);
            }
        }
    }
}
