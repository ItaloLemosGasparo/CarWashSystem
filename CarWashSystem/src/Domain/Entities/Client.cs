using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CarWashSystem.src.Domain.Entities.Phone;

namespace CarWashSystem.src.Domain.Entities
{
    public class Client : BaseEntity
    {
        public Client()
        {
            ClientPhones = new List<ClientPhone>();
            Vehicles = new List<Vehicle>();
        }


        [Required, MaxLength(200)]
        public string Name { get; set; }


        [MaxLength(200), EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }


        // 1:N
        public virtual ICollection<ClientPhone> ClientPhones { get; set; }


        // 1:N
        public virtual ICollection<Vehicle> Vehicles { get; set; }
    }
}
