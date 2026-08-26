using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace WarehouseAPI.Models
{
    [Table("Produtos")]

    public class Produto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string CodigoDebarras { get; set; }
        public double Preco { get; set; }
        public int QuantidadeEmEstoque { get; set; }
    }
}