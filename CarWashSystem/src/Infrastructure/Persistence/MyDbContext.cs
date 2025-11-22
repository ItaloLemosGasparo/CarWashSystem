using CarWashSystem.src.Domain.Entities;
using CarWashSystem.src.Domain.Entities.Configurations;
using CarWashSystem.src.Domain.Entities.Configurations.Phones;
using CarWashSystem.src.Domain.Entities.Configurations.Service;
using CarWashSystem.src.Domain.Entities.Phone;
using CarWashSystem.src.Domain.Entities.Service;
using System.Data.Entity;

namespace CarWashSystem.src.Infrastructure.Persistence
{
    public class MyDbContext : DbContext
    {
        public MyDbContext() : base("name=DefaultConnection")
        {
            // Inicializador para testes
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<MyDbContext>());
        }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<ClientPhone> ClientPhones { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<ServiceItem> ServiceItems { get; set; }
        public DbSet<ServiceOrder> ServiceOrders { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserPhone> UserPhones { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Aplica todas as configurações de Fluent API
            modelBuilder.Configurations.Add(new ClientConfiguration());
            modelBuilder.Configurations.Add(new VehicleConfiguration());
            modelBuilder.Configurations.Add(new ClientPhoneConfiguration());
            modelBuilder.Configurations.Add(new ServiceConfiguration());
            modelBuilder.Configurations.Add(new ServiceItemConfiguration());
            modelBuilder.Configurations.Add(new ServiceOrderConfiguration());
            modelBuilder.Configurations.Add(new PaymentConfiguration());
            modelBuilder.Configurations.Add(new UserConfiguration());
            modelBuilder.Configurations.Add(new UserPhoneConfiguration());
        }
    }

}
