using DXLib.Cohort;
using DXLib.User;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DxLib.DbCaching
{
    public class DbUser : DbCommon<UserRecord>
    {
        public DbUser(IOptions<DbCacheOptions> options) : base(options)
        {
        }
        public async Task<UserRecord> GetUser(string username)
        {
            var filter = Builders<UserRecord>.Filter.Eq("Username", username);
            return await base.BaseGetOneAsync(filter);
        }
        public async Task Update(UserRecord userRecord)
        {
            var filter = Builders<UserRecord>.Filter.Eq("Username", userRecord.Username);
            await base.BaseUpdateAsync(filter, userRecord);
            return;
        }
        public async Task Delete(UserRecord userRecord)
        {
            var filter = Builders<UserRecord>.Filter.Eq("Username", userRecord.Username);
            await base.BaseDeleteAsync(filter);
            return;
        }
        public async Task Delete(string Username)
        {
            var filter = Builders<UserRecord>.Filter.Eq("Username", Username);
            await base.BaseDeleteAsync(filter);
            return;
        }
    }
}
