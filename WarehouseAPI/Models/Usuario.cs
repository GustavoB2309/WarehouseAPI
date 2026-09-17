using System.ComponentModel.DataAnnotations;

namespace WarehouseAPI.Models
{
    public class Usuario
    {
        public int Id { get; set;  }
        [Required(ErrorMessage = "O Login é obrigatório.")]
        public string Login { get; set; }
        [Required(ErrorMessage = "O hash da senha é obrigatório.")]
        public string SenhaHash { get; set; }
        [Required(ErrorMessage = "O cargo é obrigatório.")]
        public string Cargo { get; set; }
    }
}
