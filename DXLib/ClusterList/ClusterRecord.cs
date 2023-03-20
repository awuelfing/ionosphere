using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXLib.ClusterList
{
    public class ClusterRecord
    {
        public DateTime RetrievalDate { get; set; } = DateTime.UtcNow;
        public List<ClusterNode>? ClusterNodes { get; set; }
    }
}
