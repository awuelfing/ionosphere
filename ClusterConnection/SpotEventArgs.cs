using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClusterConnection
{
    public class SpotEventArgs : EventArgs
    {
        public string Spotter { get; set; }
        public string Frequency { get; set; }
        public string Spottee { get; set; }
        public string Comment { get; set; }
        public string Time { get; set; }
        public SpotEventArgs(string spotter, string frequency, string spottee, string comment, string time)
        {
            Spotter = spotter;
            Frequency = frequency;
            Spottee = spottee;
            Comment = comment;
            Time = time;
        }
    }
}
