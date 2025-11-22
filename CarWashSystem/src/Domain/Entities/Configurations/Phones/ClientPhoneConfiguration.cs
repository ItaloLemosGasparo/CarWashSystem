using CarWashSystem.src.Domain.Entities.Phone;
using System.Data.Entity.ModelConfiguration;

namespace CarWashSystem.src.Domain.Entities.Configurations
{
    public class ClientPhoneConfiguration : EntityTypeConfiguration<ClientPhone>
    {
        public ClientPhoneConfiguration()
        {
            ToTable("ClientPhones");

            HasKey(clientPhone => clientPhone.Id);

            // relations

            // Relations
            HasRequired(phone => phone.Client)
                .WithMany(client => client.ClientPhones)
                .HasForeignKey(phone => phone.ClientId)
                .WillCascadeOnDelete(true);
        }
    }
}
