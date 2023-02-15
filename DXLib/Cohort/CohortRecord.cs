using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXLib.Cohort
{
    public class CohortRecord
    {
        public string Username { get; set; } = string.Empty;
        public IEnumerable<string> Cohorts { get; set; } = Enumerable.Empty<string>();
                
    }
}
