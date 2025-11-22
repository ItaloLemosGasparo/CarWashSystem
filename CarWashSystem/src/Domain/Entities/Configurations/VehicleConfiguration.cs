using System.Data.Entity.ModelConfiguration;

namespace CarWashSystem.src.Domain.Entities.Configurations
{
    public class VehicleConfiguration : EntityTypeConfiguration<Vehicle>
    {
        public VehicleConfiguration()
        {
            ToTable("Vehicles");

            HasKey(vehicle => vehicle.Id);

            // Properties
            HasIndex(vehicle => vehicle.Plate)
                .IsUnique();

            // Relations
            HasRequired(vehicle => vehicle.Client)
                .WithMany(client => client.Vehicles)
                .HasForeignKey(vehicle => vehicle.ClientId)
                .WillCascadeOnDelete(true);
        }
    }
}
