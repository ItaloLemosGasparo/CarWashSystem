using CarWashSystem.src.Domain.Entities.Phone;
using System.Data.Entity.ModelConfiguration;

namespace CarWashSystem.src.Domain.Entities.Configurations.Phones
{
    public class UserPhoneConfiguration : EntityTypeConfiguration<UserPhone>
    {
        public UserPhoneConfiguration()
        {
            ToTable("UserPhones");

            HasKey(userPhone => userPhone.Id);

            // properties

            // relations
            HasRequired(userPhone => userPhone.User)
                .WithMany(user => user.UserPhones)
                .HasForeignKey(userPhone => userPhone.UserId)
                .WillCascadeOnDelete(true);
        }
    }
}
