using CarWashSystem.src.Domain.Entities.Service;
using System.Data.Entity.ModelConfiguration;

namespace CarWashSystem.src.Domain.Entities.Configurations
{
    public class ServiceOrderConfiguration : EntityTypeConfiguration<ServiceOrder>
    {
        public ServiceOrderConfiguration()
        {
            ToTable("ServiceOrders");

            HasKey(serviceOrder => serviceOrder.Id);

            // properties

            // relations
            HasRequired(serviceOrder => serviceOrder.Vehicle)
                .WithMany(vehicle => vehicle.ServiceOrders)
                .HasForeignKey(serviceOrder => serviceOrder.VehicleId)
                .WillCascadeOnDelete(false);
        }
    }
}
