using System.ComponentModel.DataAnnotations;

namespace CarWashSystem.src.Domain.Entities.Phone
{
    public class UserPhone : BaseEntity
    {
        [Range(11, 99, ErrorMessage = "O DDD deve ser um número entre 11 e 99.")]
        public int DDD { get; set; }


        [Required(ErrorMessage = "O Número é obrigatório.")]
        [StringLength(9, MinimumLength = 8, ErrorMessage = "O número deve ter 8 ou 9 dígitos.")]
        [RegularExpression(@"^[0-9]{8,9}$", ErrorMessage = "Formato de número inválido.")]
        public string Number { get; set; }


        public int UserId { get; set; }
        public virtual User User { get; set; }
    }
}
