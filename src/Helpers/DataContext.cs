using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RegistrationAPI.Entities;


namespace RegistrationAPI.Helpers
{
    public class DataContext : DbContext
    {
     // protected readonly IConfiguration Configuration;


        public DataContext(DbContextOptions<DataContext>opt) : base(opt)
        {

        }
        // public DataContext(IConfiguration configuration)
        // {
        //     Configuration = configuration;
        // }

        // protected override void OnConfiguring(DbContextOptionsBuilder options)
        // {
        //     // connect to sql server database
        //     //options.UseMySql(Configuration.GetConnectionString("mysqlconnection"));
        //    options.UseSqlServer(Configuration.GetConnectionString("SqlSvrConn"));

        // }

        public DbSet<User> Users { get; set; }
    }
}