using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace CarWashSystem.src.Domain.Entities.Configurations
{
    public class ClienteConfiguration : EntityTypeConfiguration<Client>
    {
        public ClienteConfiguration()
        {
            ToTable("Clients");

            HasKey(client => client.Id);

            // Properties
            

            // Relations
            HasMany(client => client.PhoneNumbers)
                .WithRequired(phones => phones.Client)
                .HasForeignKey(phones => phones.ClientId)
                .WillCascadeOnDelete(true); // * Cascade delete *

            HasMany(c => c.Vehicles) 
                .WithRequired(vehicles => vehicles.Client)
                .HasForeignKey(vehicles => vehicles.ClientId)
                .WillCascadeOnDelete(true); // * Cascade delete *
        }
    }
}
