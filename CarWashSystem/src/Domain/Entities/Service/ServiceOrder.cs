using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CarWashSystem.src.Domain.Enums;

namespace CarWashSystem.src.Domain.Entities.Service
{
    public class ServiceOrder : BaseEntity, IValidatableObject
    {
        public ServiceOrder()
        {
            ServiceItems = new List<ServiceItem>();
            CreatedAt = DateTime.UtcNow;
            ServiceOrderStatus = ServiceOrderStatus.Waiting;
            EntryDate = DateTime.UtcNow;
        }


        public ServiceOrder(Vehicle vehicle) : this()
        {
            Vehicle = vehicle ?? throw new ArgumentNullException(nameof(vehicle));
            VehicleId = vehicle.Id;
        }


        public int VehicleId { get; set; }
        public virtual Vehicle Vehicle { get; set; }


        public DateTime EntryDate { get; set; }
        public DateTime? LeaveDate { get; set; }


        public virtual ICollection<ServiceItem> ServiceItems { get; set; }


        public ServiceOrderStatus ServiceOrderStatus { get; set; }


        public int? PaymentId { get; set; }
        public virtual Payment Payment { get; set; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (LeaveDate.HasValue && LeaveDate.Value < EntryDate)
                yield return new ValidationResult("A data de saída não pode ser anterior à entrada.", new[] { nameof(LeaveDate), nameof(EntryDate) });
        }
    }
}
