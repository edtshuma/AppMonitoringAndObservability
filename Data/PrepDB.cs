
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PlatformService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlatformService.Data
{
    public static class PrepDB
    {
        public static void PrepPopulation(IApplicationBuilder app, bool isProd)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>(),isProd);
            }
        }
        public static void SeedData(AppDbContext applicationDbContext, bool isProd)
        {
           

                   if (isProd) {
                         System.Console.WriteLine("Applying Migrations ...");     
                  try 
                  {
                   
                      applicationDbContext.Database.Migrate();
                  }
                  catch(Exception ex) {

                    System.Console.WriteLine($"----> Could not run migrations : { ex.Message }"); 
                  }
               
             }
           // if (applicationDbContext.Database.ProviderName != "Microsoft.EntityFrameworkCore.InMemory")
           //{
            //   applicationDbContext.Database.Migrate();
           //}           
        
            if (!applicationDbContext.Platforms.Any())
            {
                System.Console.WriteLine("Adding Data ... seeding");

                     applicationDbContext.Platforms.AddRange(
                     new Platform { Name = "Dot Net" , Publisher ="Microsoft", Cost="Free"},
                     new Platform { Name = "Linux", Publisher ="Ubuntu", Cost="Free" },
                     new Platform { Name = "Kubernetes", Publisher ="CNCF", Cost="Free" },
                     new Platform { Name = "SQL Express", Publisher ="Microsoft", Cost="Free" }

                );
                applicationDbContext.SaveChanges();
            }
            else
            {
                System.Console.WriteLine("System already has data... not seeding");
            }
        }
    }
}
