using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using App.Metrics.Formatters.Prometheus;

namespace RegistrationAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                // .UseMetricsWebTracking()
                // .UseMetricsEndpoints( options =>
                //     {
                //       options.MetricsTextEndpointOutputFormatter = new MetricsPrometheusTextOutputFormatter();
                //       options.MetricsEndpointOutputFormatter = new MetricsPrometheusProtobufOutputFormatter();
                //       options.EnvironmentInfoEndpointEnabled = false;
                //     }
                // )
             
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls("http://localhost:80/");

                });
    }
}
