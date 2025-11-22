using System.Data.Entity.ModelConfiguration;

namespace CarWashSystem.src.Domain.Entities.Configurations
{
    public class PaymentConfiguration : EntityTypeConfiguration<Payment>
    {
        public PaymentConfiguration()
        {
            ToTable("Payments");

            HasKey(payment => payment.Id);

            // properties


            // relations
            HasRequired(payment => payment.ServiceOrder)
                .WithOptional(serviceOrder => serviceOrder.Payment)
                .WillCascadeOnDelete(true);
        }
    }
}