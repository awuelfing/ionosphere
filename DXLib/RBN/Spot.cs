using DXLib.CtyDat;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DXLib.RBN
{
    public class Spot
    {
        public string Spotter { get; set; } = string.Empty;
        public string Frequency { get; set; } = string.Empty;
        public string Spottee { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
        public string Time { get; set; } = string.Empty;
        public DateTime ReceivedDateTime { get; set; }
        public int Signal { get; set; } = 0;
        public int WPM { get; set; } = 0;
        public DateTimeOffset? ReportDateTime { get; set; } = DateTime.UtcNow;
        public string Band { get; set; } = string.Empty;
        public RBNNode? SpotterStationInfo { get; set; }
        public CtyResult? SpottedStationInfo { get; set; }
    }
}
