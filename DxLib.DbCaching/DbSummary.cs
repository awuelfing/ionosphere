using DXLib.RBN;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DxLib.DbCaching
{
    public class DbSummary : DbCommon<SummaryRecord>
    {
        public DbSummary(IOptions<DbCacheOptions> options) : base(options)
        {
        }
    }
}
