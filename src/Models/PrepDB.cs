using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RegistrationAPI.Entities;
using RegistrationAPI.Services;
using RegistrationAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RegistrationAPI.Helpers;

namespace RegistrationAPI.Models
{
    public static class PrepDB
    {
        public static void PrepPopulation(IApplicationBuilder app, bool isProd)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                SeedData(serviceScope.ServiceProvider.GetService<DataContext>(), isProd);
            }
        }
        public static void SeedData(Helpers.DataContext applicationDbContext, bool isProd)
        {

            if(isProd) {

            System.Console.WriteLine("Attempting to apply Migrations ...");
           
            try{
                 applicationDbContext.Database.Migrate();
            }
            catch(Exception ex){
                
                Console.WriteLine($"---> Could not run migrations: {ex.Message}");
            }
            }
           
            if (!applicationDbContext.Users.Any())
            {
                System.Console.WriteLine("Adding Data ... seeding");
                
                User seeduser = new User();
                var password = "1thu@111";
             
                byte[] passwordHash, passwordSalt;
                UserService.CreatePasswordHash(password, out passwordHash, out passwordSalt);
                seeduser.PasswordHash = passwordHash;
                seeduser.PasswordSalt = passwordSalt;

                applicationDbContext.Users.AddRange(
                   new User { LastName = "Tshuma" ,FirstName="Thelma", Username="Gold", PasswordHash=seeduser.PasswordSalt, PasswordSalt=  seeduser.PasswordSalt }       
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
