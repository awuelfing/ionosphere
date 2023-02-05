using DxLib.DbCaching;
using DXLib.HamQTH;
using Microsoft.Extensions.Options;

namespace DXws
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.Configure<DbCacheOptions>(builder.Configuration.GetSection(DbCacheOptions.DbCache));
            builder.Services.Configure<HamQTHOptions>(builder.Configuration.GetSection(HamQTHOptions.HamQTH));
            builder.Services.AddScoped<HamQTHGeo, HamQTHGeo>();
            builder.Services.AddScoped<QthLookup>(s => new DbCache(s.GetRequiredService<IOptions<DbCacheOptions>>()) {Lower = s.GetRequiredService<HamQTHGeo>() });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();


            app.Run();
        }
    }
}