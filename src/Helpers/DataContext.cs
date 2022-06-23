using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RegistrationAPI.Entities;


namespace RegistrationAPI.Helpers
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext>opt) : base(opt)
        {

        }
        public DbSet<User> Users { get; set; }
    }
}