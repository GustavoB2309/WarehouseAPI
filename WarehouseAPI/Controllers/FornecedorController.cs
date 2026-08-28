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

    }
}
