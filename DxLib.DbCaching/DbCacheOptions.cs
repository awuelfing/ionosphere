using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DxLib.DbCaching
{
    public class DbCacheOptions
    {
        public const string DbCache = "DbCache";
        public string ConnectionString { get; set; } = string.Empty;
        public string Database { get; set; } = string.Empty;
        public string Collection { get; set; } = string.Empty;
        public string QueueCollection { get; set; } = string.Empty;
        public int MaxCacheAge { get; set; } = 0;
    }
}
