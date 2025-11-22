using System.ComponentModel.DataAnnotations;

namespace CarWashSystem.src.Domain.Entities.Service
{
    public class Service : BaseEntity
    {
        [Required(ErrorMessage = "O nome do serviço é obrigatório.")]
        [MaxLength(200, ErrorMessage = "O nome não pode exceder 200 caracteres.")]
        public string Name { get; set; }


        [MaxLength(500, ErrorMessage = "A descrição não pode exceder 500 caracteres.")]
        public string Description { get; set; }


        [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "O preço base deve ser positivo.")]
        [DataType(DataType.Currency)]
        [Required(ErrorMessage = "O preço base é obrigatório.")]
        public decimal BasePrice { get; set; }
    }
}
