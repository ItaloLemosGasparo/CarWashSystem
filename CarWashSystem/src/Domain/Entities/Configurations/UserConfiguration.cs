using System.Data.Entity.ModelConfiguration;

namespace CarWashSystem.src.Domain.Entities.Configurations
{
    public class UserConfiguration : EntityTypeConfiguration<User>
    {
        public UserConfiguration()
        {
            ToTable("Users");

            HasKey(c => c.Id);

            // Properties


            // Relations
            HasMany(user => user.PhoneNumbers)
                .WithRequired(phone => phone.User)
                .HasForeignKey(phone => phone.UserId)
                .WillCascadeOnDelete(true); // * Cascade delete *

            HasMany(user => user.ServiceItems)
                .WithRequired(serviceItem => serviceItem.User)
                .HasForeignKey(serviceItem => serviceItem.UserId)
                .WillCascadeOnDelete(false); // * Cascade delete *
        }
    }
}
