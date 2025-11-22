using CarWashSystem.src.Domain.Entities.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CarWashSystem.src.Domain.Entities
{
    public class Vehicle : BaseEntity, IValidatableObject
    {
        public Vehicle()
        {
            ServiceOrders = new List<ServiceOrder>();
        }


        public Vehicle(Client client, ServiceOrder serviceOrder)
        {
            Client = client ?? throw new ArgumentNullException(nameof(client));
            ClientId = client.Id;
        }


        public int ClientId { get; set; } // FK
        public virtual Client Client { get; set; }


        [Required(ErrorMessage = "O campo Placa é obrigatório.")]
        [StringLength(7, MinimumLength = 7, ErrorMessage = "A placa deve ter 7 caracteres.")]
        [RegularExpression(@"^[A-Z]{3}[0-9][0-9A-Z][0-9]{2}$", ErrorMessage = "O formato da placa é inválido. Utilize o padrão LLLNDDD ou LLLNXDD (onde L=Letra, N=Número, X=Letra/Número).")]
        public string Plate { get; set; }


        [MaxLength(100)]
        public string Model { get; set; }


        [MaxLength(100)]
        public string Brand { get; set; }


        [Range(1900, 3000, ErrorMessage = "Ano de Fabricação inválido.")]
        [Required]
        public int YearFab { get; set; }


        [Range(1900, 3000, ErrorMessage = "Ano do Modelo inválido.")]
        [Required]
        public int YearModel { get; set; }

        public virtual ICollection<ServiceOrder> ServiceOrders { get; set; }


        // Business validations that can't be expressed as attributes
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (YearModel < YearFab)
                yield return new ValidationResult("O Ano do Modelo não pode ser menor que o Ano de Fabricação.", new[] { nameof(YearModel), nameof(YearFab) });


            // check reasonable future year
            if (YearFab > DateTime.UtcNow.Year + 1 || YearModel > DateTime.UtcNow.Year + 1)
                yield return new ValidationResult("Ano fora do intervalo esperado.", new[] { nameof(YearFab), nameof(YearModel) });
        }
    }
}
