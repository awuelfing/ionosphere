using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXLib.RBN
{
    public class SummaryRecord
    {
        public DateTime RecordStartUtc { get; set; } = DateTime.MinValue;
        public DateTime RecordEndUtc { get; set; } = DateTime.MaxValue;
        public string TaskRunnerHost { get; set; } = string.Empty;
        public Dictionary<string,List<string>>? Activity { get; set; }
    }
}
