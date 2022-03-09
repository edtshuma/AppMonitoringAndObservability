
using System.ComponentModel;

using System.Reflection;

using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using PlatformService.Data;
using PlatformService.Models;
using PlatformService.SyncDataService.Http;
using PlatformsService.SyncDataService.Http;
using PlatformService.AsyncDataServices;

namespace PlatformService
{
    public class Startup
    {
        private readonly IWebHostEnvironment _env;
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }

        public IConfiguration Configuration { get; }

      
        public void ConfigureServices(IServiceCollection services )
        {

          if (_env.IsProduction()) {
            Console.WriteLine("Using SqlServerDB");
            services.AddDbContext<AppDbContext>(opt =>
            opt.UseSqlServer(Configuration.GetConnectionString("PlatformsConn")));

          }
          else {
              Console.WriteLine("Using In-MemoryDB");
            services.AddDbContext<AppDbContext>(opt =>
            opt.UseInMemoryDatabase("InMem"));

          }
           
            //register interface, & its concrete implementation
            //if client asks for IPlatformRepo, give them PlatformRepo
            services.AddScoped<IPlatformRepo, PlatformRepo>();
            services.AddHttpClient<ICommandDataClient, CommandDataClient>();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddControllers();
            services.AddSingleton<IMessageBusClient, MessageBusClient>();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PlatformService", Version = "v1" });
            });
 
            Console.WriteLine($"--> CommandService Endpoint {Configuration["CommandService"]}");
        }

        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PlatformService v1"));
            }

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

          PrepDB.PrepPopulation(app, env.IsProduction());
        }
    }
}
