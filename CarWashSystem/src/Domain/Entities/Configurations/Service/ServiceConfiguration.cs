using System.Data.Entity.ModelConfiguration;

namespace CarWashSystem.src.Domain.Entities.Configurations
{
    public class ServiceConfiguration : EntityTypeConfiguration<Entities.Service.Service>
    {
        public ServiceConfiguration()
        {
            ToTable("Services");

            HasKey(service => service.Id);

            // properties

            // relations
        }
    }
}
