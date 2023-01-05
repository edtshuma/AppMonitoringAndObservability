using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Prometheus;
using RegistrationAPI.AppMetricsFiles;
using RegistrationAPI.Helpers;
using RegistrationAPI.Middleware;
using RegistrationAPI.Models;
using RegistrationAPI.Services;
using System;
using System.Text;
using System.Threading.Tasks;

namespace RegistrationAPI
{
    public class Startup
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;

        public Startup(IWebHostEnvironment env,IConfiguration configuration)
        {
            _env = env;
            _configuration = configuration;
        }

        //public IConfiguration Configuration { get; }

     
        public void ConfigureServices(IServiceCollection services)
        {
             
            if (_env.IsProduction()){
                 Console.WriteLine ("--> Using SqlSvr");
                services.AddDbContext<DataContext>(opt =>
                opt.UseSqlServer(_configuration.GetConnectionString("SqlSvrConn")));
            }           
            else {
                 Console.WriteLine ("--> Using SqlLite");
                 services.AddDbContext<DataContext>(opt =>
                 opt.UseInMemoryDatabase(("InMem")));
            }
                                             
            services.AddCors();
            services.AddControllers();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // strongly-typed appsettings 
            var appSettingsSection = _configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);
            services.AddSingleton<MetricReporter>();
            //jwt
            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        var userService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();
                        var userId = int.Parse(context.Principal.Identity.Name);
                        var user = userService.GetById(userId);
                        if (user == null)
                        {
                            // return unauthorized if user no longer exists
                            context.Fail("Unauthorized");
                        }
                        return Task.CompletedTask;
                    }
                };
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            // configure DI for application services
          services.AddScoped<IUserService, UserService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //dataContext.Database.Migrate();
            app.UseRouting();

            // global cors policy
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseAuthentication();
            app.UseAuthorization();
            //place b4 app.UseEndpoints() to avoid losing some metrics
            app.UseMetricServer();
            app.UseMiddleware<ResponseMetricMiddleware>();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
            //
             PrepDB.PrepPopulation(app, env.IsProduction());
           
        }
    }
}
