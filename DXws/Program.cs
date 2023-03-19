using DxLib.DbCaching;
using DXLib.HamQTH;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Serilog;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json.Serialization;

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

            byte[] key = Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("JwtKey") ?? "");
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
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/account/login";
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(200);
                    options.SlidingExpiration = true;
                    options.AccessDeniedPath = "/web/index";
                });
            /*
            builder.Services.AddAuthorization(options =>
            {
                var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(
                    JwtBearerDefaults.AuthenticationScheme,
                    CookieAuthenticationDefaults.AuthenticationScheme);
                defaultAuthorizationPolicyBuilder =
                    defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser();
                options.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();
            });*/
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Both";
                options.DefaultChallengeScheme = "Both";
            })
            .AddPolicyScheme("Both", "Both", options =>
             {
                 options.ForwardDefaultSelector = context =>
                 {
                     string? authorization = context.Request.Headers[HeaderNames.Authorization];
                     if (!string.IsNullOrEmpty(authorization) && authorization.StartsWith("Bearer "))
                     {
                         return JwtBearerDefaults.AuthenticationScheme;
                     }
                     return CookieAuthenticationDefaults.AuthenticationScheme;
                 };
             });

            //builder.Services.AddControllers();
            builder.Services.AddControllersWithViews(x =>
            {
                x.ReturnHttpNotAcceptable = true;
                x.CacheProfiles.Add("CacheLong", new() { Duration = 600, VaryByQueryKeys = new[] { "*" }  });
                x.CacheProfiles.Add("CacheShort", new() { Duration = 60, VaryByQueryKeys = new[] { "*" } });
            })
            .AddNewtonsoftJson(y =>
            {
                y.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                y.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            })
            .AddXmlDataContractSerializerFormatters();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddResponseCaching();

            builder.Services.Configure<DbCacheOptions>(builder.Configuration.GetSection(DbCacheOptions.DbCache));
            builder.Services.Configure<HamQTHOptions>(builder.Configuration.GetSection(HamQTHOptions.HamQTH));
            builder.Services.AddScoped<HamQTHGeo, HamQTHGeo>();
            //builder.Services.AddScoped<QthLookup>(s => new DbCache(s.GetRequiredService<IOptions<DbCacheOptions>>()) { Lower = s.GetRequiredService<HamQTHGeo>() });
            builder.Services.AddScoped<IQthLookup>(s => new DbCache(s.GetRequiredService<IOptions<DbCacheOptions>>()) { _qthLookup = s.GetRequiredService<HamQTHGeo>() });
            builder.Services.AddScoped<DbQueue, DbQueue>();
            builder.Services.AddScoped<DbSpots, DbSpots>();
            builder.Services.AddScoped<DbCohort, DbCohort>();
            builder.Services.AddScoped<DbUser, DbUser>();
            builder.Services.AddScoped<DbSummary, DbSummary>();

            var app = builder.Build();

            app.UseAuthentication();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseAuthorization();

            app.UseResponseCaching();
            app.MapControllers();


            app.Run();
        }
    }
}