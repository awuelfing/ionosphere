﻿using DXLib.HamQTH;
using DXLib.RBN;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DxLib.DbCaching
{
    public class DbCommon<T>
    {
        protected readonly DbCacheOptions _options;
        protected MongoClientSettings? _settings;
        protected MongoClient? _mongoClient;
        protected IMongoDatabase? _mongoDatabase;
        protected IMongoCollection<T>? _mongoCollection;
        protected bool _initialized = false;
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
            BsonClassMap.RegisterClassMap<T>(cm =>
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

        public async Task BaseStoreOneAsync(T input)
        {
            if (!this._initialized)
            {
                this.Initialize();
            }
            await _mongoCollection!.InsertOneAsync(input);

            return;
        }
        public async Task<T> BaseGetOneAsync(FilterDefinition<T> filter)
        {
            if (!this._initialized)
            {
                this.Initialize();
            }
            var results = await _mongoCollection!.FindAsync(filter);
            return await results.FirstOrDefaultAsync();
        }
        public async Task<List<T>> BaseGetManyAsync(FilterDefinition<T> filter)
        {
            if (!this._initialized)
            {
                this.Initialize();
            }
            var results = await _mongoCollection!.FindAsync(filter);
            return await results.ToListAsync();
        }
        public async Task BaseUpdateAsync(FilterDefinition<T> filter,T t)
        {
            if (!this._initialized)
            {
                this.Initialize();
            }
            await _mongoCollection!.DeleteManyAsync(filter);
            await _mongoCollection!.InsertOneAsync(t);
        }
        public async Task BaseDeleteAsync(FilterDefinition<T> filter)
        {
            if (!this._initialized)
            {
                this.Initialize();
            }
            await _mongoCollection!.DeleteManyAsync(filter);
        }
        public async Task BaseDeleteAllAsync()
        {
            if (!this._initialized)
            {
                this.Initialize();
            }
            if (typeof(T).Name != nameof(Spot))
            {
                throw new Exception("Maybe not...");
            }
            await _mongoCollection!.DeleteManyAsync(Builders<T>.Filter.Empty);
        }
        public IMongoQueryable<T> BaseGetIQueryable()
        {
            if (!this._initialized)
            {
                this.Initialize();
            }
            return _mongoCollection.AsQueryable();
        }
    }
}
