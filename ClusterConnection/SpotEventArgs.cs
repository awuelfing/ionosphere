using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DxLib;
using DXLib.RBN;

namespace ClusterConnection
{
    public class SpotEventArgs : EventArgs
    {
        private static string _signalExtraction = @"(?<Signal>\d+)\sdB";
        public string Spotter { get; set; }
        public string Frequency { get; set; }
        public string Spottee { get; set; }
        public string Comment { get; set; }
        public string Time { get; set; }
        public DateTime ReceivedDateTime { get; set; }
        public SpotEventArgs(string spotter, string frequency, string spottee, string comment, string time)
        {
            Spotter = spotter;
            Frequency = frequency;
            Spottee = spottee;
            Comment = comment;
            Time = time;
            ReceivedDateTime = DateTime.UtcNow;
        }
        public int Signal
        { 
            get 
            {
                Match match = Regex.Match(this.Comment, _signalExtraction, RegexOptions.Multiline);
                return match.Groups["Signal"].Success ? int.Parse(match.Groups["Signal"].Value) : 0;
            }
            set { } 
        }
        public DateTimeOffset? ReportDateTime
        {
            get
            {
                DateTimeOffset dateTimeOffset;
                if(DateTimeOffset.TryParseExact(this.Time,"HHmm",CultureInfo.InvariantCulture.DateTimeFormat,DateTimeStyles.AssumeUniversal,out dateTimeOffset))
                {
                    return dateTimeOffset;
                }
                return null;
            }
            set
            {

            }
        }
        public string Band
        {
            get
            {
                if (this.Frequency.StartsWith("18") && (this.Frequency.Length == 4 || this.Frequency.IndexOf('.') == 4)) return "160m";
                if (this.Frequency.StartsWith("3") && (this.Frequency.Length == 4 || this.Frequency.IndexOf('.') == 4)) return "80m";
                if (this.Frequency.StartsWith("7") && (this.Frequency.Length == 4 || this.Frequency.IndexOf('.') == 4)) return "40m";
                if (this.Frequency.StartsWith("14")) return "20m";
                if (this.Frequency.StartsWith("21")) return "15m";
                if (this.Frequency.StartsWith("28") || this.Frequency.StartsWith("29")) return "10m";
                if (this.Frequency.StartsWith("24")) return "12m";
                if (this.Frequency.StartsWith("18")) return "17m";
                if (this.Frequency.StartsWith("10")) return "10m";
                return "unknown";
            }
            set
            {

            }
        }
        public string FormatSpot()
        {
            return $"[{Math.Round((DateTime.Now-this.ReceivedDateTime).TotalSeconds)}s ago] {this.Spotter} saw {this.Spottee} on {this.Band}";
        }
        public static Spot ConvertSpot(SpotEventArgs e)
        {
            return new Spot()
            {
                Spotter = e.Spotter,
                Frequency = e.Frequency,
                Spottee = e.Spottee,
                Comment = e.Comment,
                Time = e.Time,
                ReceivedDateTime = e.ReceivedDateTime,
                Signal = e.Signal,
                Band = e.Band,
                ReportDateTime = e.ReportDateTime
            };
        }
        public Spot AsSpot()
        {
            return ConvertSpot(this);
        }
    }
}



