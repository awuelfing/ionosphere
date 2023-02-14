using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace DxLib.DbCaching
{
    public class DbQueue : DbCommon<DbQueueRecord>
    {
        public DbQueue(IOptions<DbCacheOptions> options) : base(options)
        {
        }

        public DbQueue(DbCacheOptions options) : base(options)
        {
        }

        public async Task EnqueueAsync(string callsign, int count)
        {
            if (!this._initialized)
            {
                this.Initialize();
            }

            var filter = Builders<DbQueueRecord>.Filter.Eq("Callsign", callsign.ToUpper());
            var results = await _mongoCollection.FindAsync(filter);
            var result = await results.FirstOrDefaultAsync();
            if (result == null)
            {
                DbQueueRecord record = new DbQueueRecord() {
                    Callsign = callsign,
                    RequestedCount = count
                };
                await _mongoCollection!.InsertOneAsync(record);
                return;
            }
            else
            {
                var update = Builders<DbQueueRecord>.Update.Set("RequestedCount", result.RequestedCount+count);
                await _mongoCollection!.UpdateOneAsync(filter, update);
            }
            return;
        }
        public async Task<DbQueueRecord?> DequeueAsync(bool peek = false)
        {
            if (!this._initialized)
            {
                this.Initialize();
            }

            var filter = Builders<DbQueueRecord>.Filter.Empty;
            var sort = Builders<DbQueueRecord>.Sort.Descending("RequestedCount");
            var options = new FindOptions<DbQueueRecord>() { Sort= sort };
            var cursor = await _mongoCollection!.FindAsync(filter, options);
            var result = await cursor.FirstOrDefaultAsync();
            if (!peek && result != null)
            {
                filter = Builders<DbQueueRecord>.Filter.Eq("Callsign", result.Callsign);
                await _mongoCollection.DeleteOneAsync(filter);
            }
            return result;
        }
    }
}
