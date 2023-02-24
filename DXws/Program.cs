using DxLib.DbCaching;
using DXLib.HamQTH;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Serilog;
using Microsoft.Extensions.Configuration;

namespace DXws
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .CreateLogger();

            builder.Host.UseSerilog();

            byte[] key = Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("JwtKey")??"");
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = "me",
                        ValidAudience = "me",
                        IssuerSigningKey = new SymmetricSecurityKey(key)
                    };
                });
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.Configure<DbCacheOptions>(builder.Configuration.GetSection(DbCacheOptions.DbCache));
            builder.Services.Configure<HamQTHOptions>(builder.Configuration.GetSection(HamQTHOptions.HamQTH));
            builder.Services.AddScoped<HamQTHGeo, HamQTHGeo>();
            //builder.Services.AddScoped<QthLookup>(s => new DbCache(s.GetRequiredService<IOptions<DbCacheOptions>>()) { Lower = s.GetRequiredService<HamQTHGeo>() });
            builder.Services.AddScoped<IQthLookup>(s => new DbCache(s.GetRequiredService<IOptions<DbCacheOptions>>()) { _qthLookup = s.GetRequiredService<HamQTHGeo>() });
            builder.Services.AddScoped<DbQueue, DbQueue>();
            builder.Services.AddScoped<DbSpots, DbSpots>();
            builder.Services.AddScoped<DbCohort,DbCohort>();
            builder.Services.AddScoped<DbUser, DbUser>();

            var app = builder.Build();

            app.UseAuthentication();

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