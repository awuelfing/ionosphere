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
        public int QueueUploaderDelay { get; set; } = 500;
        public bool EnableQueueResolver { get; set; } = false;
        public int ResolverDelay { get; set; } = 5000;
        public bool EnableKeepAlive { get; set; } = false;
        public int KeepAliveDelay { get; set; } = 120000;
        public bool EnableSpotPurge { get; set; } = false;
        public int SpotPurgeDelay { get; set; } = 180000;
        public int SpotPurgeAgeMinutes { get; set; } = 6 * 60;
        public bool EnableSpotUpload { get; set; } = false;
        public bool EnableStatusReport { get; set; } = false;
        public int StatusReportDelay { get; set; } = 10000;
        public bool EnableSummaryUpload { get; set; } = false;
        public int SummaryUploadFrequencySeconds { get; set; } = 300;
        public bool EnableClusterReport { get; set; } = false;
        public string[] Users { get; set; } = { };
        public string ProgramHost { get; set; } = string.Empty;
    }
}
