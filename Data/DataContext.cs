using Microsoft.EntityFrameworkCore;
using ShopifyAppKyle.Models;

namespace ShopifyAppKyle.Data
{
    // Allows us to create tables from new models
    public class DataContext : DbContext
    {
        public DataContext(){}

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) 
        { 
            if (!optionsBuilder.IsConfigured) 
            { 
                optionsBuilder.UseSqlServer("Server=LAINE\\SHOPIFYSERVERDB;Database=master;MultipleActiveResultSets=true;User Id=sa;Password=Shopify01!!"); 
            } 
        } 

        // These are the classes we wnat to use as tables
        public DbSet<UserAccount> Users { get; set; }
        public DbSet<OauthState> LoginStates { get; set; }
    }
}