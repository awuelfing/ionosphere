using DXLib.HamQTH;
using DXLib.RBN;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DxLib.DbCaching
{
    public class DbCommon<T>
    {
        private readonly DbCacheOptions _options;
        private MongoClientSettings? _settings;
        private MongoClient? _mongoClient;
        private IMongoDatabase? _mongoDatabase;
        private IMongoCollection<T>? _mongoCollection;
        private bool _initialized = false;
        public DbCommon(IOptions<DbCacheOptions> options)
        {
            _options = options.Value;
        }
        public DbCommon(DbCacheOptions options)
        {
            _options = options;
        }
        static DbCommon()
        {
            BsonClassMap.RegisterClassMap<Spot>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });
        }
        public void Initialize() // moved this stuff out of the constructor
        {
            _settings = MongoClientSettings.FromConnectionString(_options.ConnectionString);
            _settings.SslSettings = new SslSettings()
            {
                EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12
            };
            _mongoClient = new MongoClient(_settings);

            _mongoDatabase = _mongoClient.GetDatabase(_options.Database);
            _mongoCollection = _mongoDatabase.GetCollection<T>($"{typeof(T).Name}Collection");
            _initialized = true;
        }

        public async Task StoreOneAsync(T input)
        {
            if (!this._initialized)
            {
                this.Initialize();
            }
            await _mongoCollection!.InsertOneAsync(input);

            return;
        }
    }
}
