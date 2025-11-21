using CarWashSystem.src.Domain.Entities.Configurations;
using System.Data.Entity;

namespace CarWashSystem.src.Infrastructure.Persistence
{
    internal class MyDbContext : DbContext
    {
        // ConnectionString not yet defined
        public MyDbContext() : base("name=ConnectionString")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Configurations:
            modelBuilder.Configurations.Add(new ClienteConfiguration());

            base.OnModelCreating(modelBuilder);
        }

        // DbSets:
        // public DbSet<Client> Clients { get; set; }
    }
}
