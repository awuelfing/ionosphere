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
        public bool EnableUploader { get; set; } = false;
        public bool EnableResolver { get; set; } = false;
        public int ResolverDelay { get; set; } = 5;
    }
}
