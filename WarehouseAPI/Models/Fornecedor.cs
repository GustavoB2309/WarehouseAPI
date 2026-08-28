using System.ComponentModel.DataAnnotations.Schema;

namespace WarehouseAPI.Models
{
    [Table("Fornecedores")]
    public class Fornecedor
    {
       
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string CNPJ { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;


    }
}
