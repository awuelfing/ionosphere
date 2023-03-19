using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClusterConnection
{
    public class ClusterClientOptions
    {
        public const string ClusterClient = "ClusterClient";
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; } = 0;
        public string Callsign { get; set; } = string.Empty;

        public int ConnectionAttempts { get; set; } = 5;
    }
}
