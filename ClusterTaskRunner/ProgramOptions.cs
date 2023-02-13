using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClusterTaskRunner
{
    public class ProgramOptions
    {
        public const string ProgramOptionName = "ProgramOptions";
        public bool EnableClusterConnection { get; set; } = false;
        public bool EnableQueueUploader { get; set; } = false;
        public bool EnableQueueResolver { get; set; } = false;
        public int ResolverDelay { get; set; } = 5;
        public bool EnableKeepAlive { get; set; } = false;
        public int KeepAliveDelay { get; set; } = 120;
        public bool EnableSpotUpload { get; set; } = false;
    }
}
