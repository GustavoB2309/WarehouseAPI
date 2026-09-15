using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WarehouseAPI.Models
{
    [Table("Produtos")]

    public class Produto
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "O nome é obrigatório.")]
        public string Nome { get; set; }
        [StringLength(20, ErrorMessage = "O código de barras não pode ter mais de 20 caracteres.")]
        public string CodigoDebarras { get; set; }
        [Range(0.01, 999999.99, ErrorMessage = "O valor deve ser maior que zero.")]
        public decimal Preco { get; set; }
        public decimal PrecoCusto { get; set; }
        public int QuantidadeEmEstoque { get; set; }
        public int FornecedorId { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public Fornecedor Fornecedor { get; set; } = null!;
    }
}