using System;
using System.ComponentModel.DataAnnotations;

namespace CarWashSystem.src.Domain.Entities.Service
{
    public class ServiceItem : BaseEntity
    {
        protected ServiceItem() { }


        public ServiceItem(Service service, User employee, decimal chargedAmount)
        {
            Service = service ?? throw new ArgumentNullException(nameof(service));
            User = employee ?? throw new ArgumentNullException(nameof(employee));
            ChargedAmount = chargedAmount;
        }


        public int ServiceOrderId { get; set; }
        public virtual ServiceOrder ServiceOrder { get; set; }


        public int ServiceId { get; set; }
        public virtual Service Service { get; set; }


        public int UserId { get; set; }
        public virtual User User { get; set; }


        [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "O valor cobrado deve ser positivo.")]
        [DataType(DataType.Currency)]
        public decimal ChargedAmount { get; set; }
    }
}
