using System.ComponentModel.DataAnnotations;

namespace CarWashSystem.src.Domain.Entities.Service
{
    public class Service : BaseEntity
    {
        [Required, MaxLength(200)]
        public string Name { get; set; }


        [MaxLength(500, ErrorMessage = "A descrição não pode exceder 500 caracteres.")]
        public string Description { get; set; }


        [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "O preço base deve ser positivo.")]
        [DataType(DataType.Currency)]
        public decimal BasePrice { get; set; }
    }
}
