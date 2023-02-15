using DXLib.Cohort;
using DXLib.HamQTH;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DxLib.DbCaching
{
    public class DbCohort : DbCommon<CohortRecord>
    {
        public DbCohort(IOptions<DbCacheOptions> options) : base(options)
        {
        }

        public DbCohort(DbCacheOptions options) : base(options)
        {
        }
        public async Task<CohortRecord?> Get(string Username)
        {
            var filter = Builders<CohortRecord>.Filter.Eq("Username", Username);
            return await base.GetOneAsync(filter);
        }
        public async Task Update(CohortRecord cohortRecord)
        {
            var filter = Builders<CohortRecord>.Filter.Eq("Username", cohortRecord.Username);
            await base.UpdateAsync(filter,cohortRecord);
            return;
        }
        public async Task Delete(CohortRecord cohortRecord)
        {
            var filter = Builders<CohortRecord>.Filter.Eq("Username", cohortRecord.Username);
            await base.DeleteAsync(filter);
            return;
        }
        public async Task Delete(string Username)
        {
            var filter = Builders<CohortRecord>.Filter.Eq("Username", Username);
            await base.DeleteAsync(filter);
            return;
        }
    }
}
