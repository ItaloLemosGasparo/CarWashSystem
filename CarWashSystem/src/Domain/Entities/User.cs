using CarWashSystem.src.Domain.Entities.Phone;
using CarWashSystem.src.Domain.Entities.Service;
using CarWashSystem.src.Domain.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CarWashSystem.src.Domain.Entities
{
    public class User : BaseEntity
    {
        public User()
        {
            UserPhones = new List<UserPhone>();
            ServiceItems = new List<ServiceItem>();
        }


        [Required, MaxLength(200)]
        public string Name { get; set; }


        [MaxLength(200), EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        public Positions Position { get; set; }


        // 1:N
        public virtual ICollection<UserPhone> UserPhones { get; set; }


        // 1:N
        public virtual ICollection<ServiceItem> ServiceItems { get; set; }
    }
}
