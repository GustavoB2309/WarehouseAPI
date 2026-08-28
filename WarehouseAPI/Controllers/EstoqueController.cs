using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using WarehouseAPI.Models;
using WarehouseAPI.Data;
using Microsoft.AspNetCore.Mvc.Formatters.Xml;

namespace WarehouseAPI.Controllers
{
    public static class EstoqueController
    {

        public static IResult AbastecerEstoque(Produto dados, AppDbContext banco)
        {
            var ProdutoExiste = banco.Produtos.Any(p => p.Nome == dados.Nome);

            if(string.IsNullOrWhiteSpace(dados.Nome)) 
            {
                Console.WriteLine($"[{DateTime.Now}] ERRO: O nome não pode estar vazio, identifique o produto que quer abastecer.");
                return Results.BadRequest(new { mensagem = "Nome vazio" });
            }

            else if (dados.QuantidadeEmEstoque <= 0) 
                {
                Console.WriteLine($"[{DateTime.Now}] ERRO: Informe a quantidade de produtos que entraram no estoque.");
                return Results.BadRequest(new { mensagem = "Informe a quantidade (ela não pode ser igual ou menor que zero" });
            }

            var produtoExistente = banco.Produtos.FirstOrDefault(p => p.Nome == dados.Nome);

            try
            {

                if (produtoExistente == null)
                {
                    banco.Produtos.Add(dados);
                    banco.SaveChanges();
                }

                else
                {
                    produtoExistente.QuantidadeEmEstoque = produtoExistente.QuantidadeEmEstoque + dados.QuantidadeEmEstoque;
                    banco.SaveChanges();
                }
            
                Console.WriteLine($"[{DateTime.Now}] INFO: Produto abastecido no estoque.");
                return Results.Ok(new { mensagem = "Produto abastecido com sucesso!" });
            

            }

            catch (Exception erro)
            {
                Console.WriteLine($"[{DateTime.Now}] CRÍTICO: Erro físico no SQL, mensagem de erro: '{erro.Message}'");
                return Results.Problem("Desculpe, o sistema está instável no momento");
            }
        }
    }
}