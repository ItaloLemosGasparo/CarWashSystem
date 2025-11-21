using CarWashSystem.src.Domain.Entities.Service;
using CarWashSystem.src.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CarWashSystem.src.Domain.Entities
{
    public class Payment : BaseEntity, IValidatableObject
    {
        protected Payment() { }


        public Payment(ServiceOrder serviceOrder)
        {
            ServiceOrder = serviceOrder ?? throw new ArgumentNullException(nameof(serviceOrder));
            ServiceOrderId = serviceOrder.Id;
        }


        [Range(0.00, (double)decimal.MaxValue, ErrorMessage = "O valor total não pode ser negativo.")]
        [DataType(DataType.Currency)]
        public decimal Total { get; set; }

        
        public PaymentMethod? PaymentMethod { get; set; }

        [Required(ErrorMessage = "O status do pagamento é obrigatório.")]
        public PaymentStatus Status { get; set; }


        public DateTime? PaymentDate { get; set; }


        public int ServiceOrderId { get; set; } // FK
        public virtual ServiceOrder ServiceOrder { get; set; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Status == PaymentStatus.Paid && !PaymentDate.HasValue)
                yield return new ValidationResult("Data de pagamento é obrigatória quando o status é 'Paid'.", new[] { nameof(PaymentDate), nameof(Status) });


            if (Total < 0)
                yield return new ValidationResult("O total não pode ser negativo.", new[] { nameof(Total) });
        }
    }
}
