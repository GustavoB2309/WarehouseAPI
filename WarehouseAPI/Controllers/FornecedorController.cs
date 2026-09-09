using WarehouseAPI.Data;
using WarehouseAPI.Models;

namespace WarehouseAPI.Controllers
{
    public static class FornecedorController
    {

        public static IResult CadastrarFornecedor(Fornecedor dados, AppDbContext banco)
        {

            if (string.IsNullOrWhiteSpace(dados.Nome))
            {
                Console.WriteLine($"[{DateTime.Now}] ERRO: O nome do fornecedor não pode estar em branco.");
                return Results.BadRequest(new { mensagem = "O nome do fornecedor não pode estar em branco." });
            }

            var fornecedorExiste = banco.Fornecedores.Any(c => c.CNPJ == dados.CNPJ);
            
            if (fornecedorExiste)
            {
                Console.WriteLine($"[{DateTime.Now}] ERRO: Esse fornecedor já existe (CNPJ).");
                return Results.BadRequest(new { mensagem = "O cnpj do fornecedor já está cadastrado no sistema." });
            }

            try 
            {
                banco.Fornecedores.Add(dados);
                banco.SaveChanges();

                return Results.Ok(new { mensagem = "Sucesso no cadastro do fornecedor." });
            }

            catch (Exception erro)
            {
                Console.WriteLine($"[{DateTime.Now}] ERRO: Nossos servidores estão com algum problema, ele é: '{erro.Message}'");
                return Results.BadRequest(new { mensagem = "Problema físico no banco de dados." });
            }
        }

        public static IResult ConsultarPorId(int id, AppDbContext banco)
        {
            var idFornecedor = banco.Fornecedores.FirstOrDefault(c => c.Id == id);

            if (idFornecedor == null)
            {
                Console.WriteLine($"[{DateTime.Now}] ERRO: O fornecedor não foi encontrado.");
                return Results.NotFound(new { mensagem = "O id pode não estar correto." });
            }

                return Results.Ok(idFornecedor);
            
        }

    }
}
