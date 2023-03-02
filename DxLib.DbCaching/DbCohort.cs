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
            return await base.BaseGetOneAsync(filter);
        }
        public async Task Update(CohortRecord cohortRecord)
        {
            var filter = Builders<CohortRecord>.Filter.Eq("Username", cohortRecord.Username);
            await base.BaseUpdateAsync(filter,cohortRecord);
            return;
        }
        public async Task Delete(CohortRecord cohortRecord)
        {
            var filter = Builders<CohortRecord>.Filter.Eq("Username", cohortRecord.Username);
            await base.BaseDeleteAsync(filter);
            return;
        }
        public async Task Delete(string Username)
        {
            var filter = Builders<CohortRecord>.Filter.Eq("Username", Username);
            await base.BaseDeleteAsync(filter);
            return;
        }
        public async Task RemoveOne(string username,string callsign)
        {
            var filter = Builders<CohortRecord>.Filter.Eq("Username", username);
            var record = await this.BaseGetOneAsync(filter);
            if (record != null)
            {
                record.Cohorts = record.Cohorts.Where(x => !x.Equals(callsign)).ToList();
            }
            else return;
            await base.BaseUpdateAsync(filter, record);
            return;
        }
    }
}
