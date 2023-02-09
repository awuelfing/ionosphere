using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DxLib.DbCaching
{
    public class DbQueueRecord
    {
        public string Callsign { get; set; } = string.Empty;
        public DateTime EnqueuedDateTime { get; set; } = DateTime.Now;
        public int RequestedCount { get; set; } = 0;
    }
}
