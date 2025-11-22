using CarWashSystem.src.Domain.Entities.Service;
using System.Data.Entity.ModelConfiguration;

namespace CarWashSystem.src.Domain.Entities.Configurations.Service
{
    public class ServiceItemConfiguration : EntityTypeConfiguration<ServiceItem>
    {
        public ServiceItemConfiguration()
        {
            ToTable("ServiceItems");

            HasKey(serviceItem => serviceItem.Id);

            // properties

            // relations
            HasRequired(si => si.Service)
               .WithMany()
               .HasForeignKey(si => si.ServiceId)
               .WillCascadeOnDelete(true);

            HasRequired(si => si.User)
                .WithMany(u => u.ServiceItems)
                .HasForeignKey(si => si.UserId)
                .WillCascadeOnDelete(false);

            HasRequired(si => si.ServiceOrder)
                .WithMany(so => so.ServiceItems)
                .HasForeignKey(si => si.ServiceOrderId)
                .WillCascadeOnDelete(true);

        }
    }
}
