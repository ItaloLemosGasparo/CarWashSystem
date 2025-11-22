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
        }
    }
}
