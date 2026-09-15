using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using WarehouseAPI.Models;
using WarehouseAPI.Data;
using Microsoft.AspNetCore.Mvc.Formatters.Xml;
using Microsoft.EntityFrameworkCore;

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

                var FornecedorExiste = banco.Fornecedores.Any(f => f.Id == dados.FornecedorId);

                if (!FornecedorExiste)
                {
                    Console.WriteLine($"[{DateTime.Now}] ERRO: O id do fornecedor não existe no banco de dados.");
                    return Results.BadRequest(new { mensagem = "O id do fornecedor não existe." });
                }

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

        public static IResult consultarPorFornecedor(int idFornecedor, AppDbContext banco)
        {

            var porFornecedor = banco.Produtos
            .Where(p => p.FornecedorId == idFornecedor)
            .ToList();

            return Results.Ok(porFornecedor);

        }

        public static IResult listarProdutos(AppDbContext banco)
        {
            var listarTudo = banco.Produtos
                .Include(p => p.Fornecedor)
                .ToList();

            return Results.Ok(listarTudo);
        }
        
        public static IResult AtualizarPreco(int id, decimal novoPreco, AppDbContext banco)
        {

            var produtoExiste = banco.Produtos.FirstOrDefault(c => c.Id == id);

            if (produtoExiste == null)
            {
                Console.WriteLine($"[{DateTime.Now}] ERRO: O id não se refere a um produto existente.");
                return Results.NotFound(new { mensagem = "O id não se refere a um produto existente." });
            }

            if (novoPreco <= 0)
            {
                Console.WriteLine($"[{DateTime.Now}] ERRO: A mudança não pode ser pra 0 ou menos.");
                return Results.BadRequest(new { mensagem = "A mudança não pode ser pra 0 ou menos." });
            }

            produtoExiste.Preco = novoPreco;
            banco.SaveChanges();

            return Results.Ok("Preço alterado.");

        }

        public static IResult DarBaixaEstoque(int id, int quantidadeSaida, AppDbContext banco)
        {
            var produtoexiste = banco.Produtos.FirstOrDefault(c => c.Id == id);

            if (produtoexiste == null)
            {
                Console.WriteLine($"[{DateTime.Now}] ERRO: O produto não existe.");
                return Results.NotFound(new { mensagem = "O produto não foi encontrado." });
            }

            if (quantidadeSaida <= 0)
            {
                Console.WriteLine($"[{DateTime.Now}] ERRO: A quantidade precisa ser maior que zero.");
                return Results.BadRequest(new { mensagem = "A quantidade precisa ser maior que zero." });
            }

            if (quantidadeSaida > produtoexiste.QuantidadeEmEstoque)
            {
                Console.WriteLine($"[{DateTime.Now}] ERRO: A quantidade informada é superior a quantidade em estoque.");
                return Results.BadRequest(new { mensagem = "A quantidade informada não pode ser superior ao estoque." });
            }

            produtoexiste.QuantidadeEmEstoque = produtoexiste.QuantidadeEmEstoque - quantidadeSaida;

            banco.SaveChanges();

            return Results.Ok("Estoque deu baixa (certa quantidade saiu do estoque).");

        }

        public static IResult DarEntradaEstoque(int id, int quantidadeEntrada, AppDbContext banco)
        {
            var produtoexiste = banco.Produtos.FirstOrDefault(c => c.Id == id);

            if (produtoexiste == null)
            {
                Console.WriteLine($"[{DateTime.Now}] ERRO: O id está incorreto.");
                return Results.NotFound(new { mensagem = "Produto não encontrado." });
            }

           if (quantidadeEntrada <= 0)
            {
                Console.WriteLine($"[{DateTime.Now}] ERRO: A quantidade inserida deve ser maior que zero.");
                return Results.BadRequest(new { mensaegm = "A quantia deve ser maior que zero." });
            }

            produtoexiste.QuantidadeEmEstoque = produtoexiste.QuantidadeEmEstoque + quantidadeEntrada;

            banco.SaveChanges();

            return Results.Ok("Tudo certo!");

        }

        public static IResult RelatorioFaturamento(AppDbContext banco)
        {
            var relatorio = banco.Produtos.Select(p => new
            {
                Id = p.Id,
                Nome = p.Nome,
                Preco = p.Preco,
                Quantidade = p.QuantidadeEmEstoque,
                ValorTotalEstoque = p.Preco * p.QuantidadeEmEstoque
            }).ToList();

            return Results.Ok(new {relatorio});
        }

        public static IResult RelatorioCurvaABC(AppDbContext banco)
        {
            var produtosOrdenados = banco.Produtos
                .Select(p => new
                {
                    id = p.Id,
                    Nome = p.Nome,
                    ValorTotalEstoque = p.QuantidadeEmEstoque * p.Preco
                })
                .OrderByDescending(p => p.ValorTotalEstoque)
                .ToList();

            var resultadoFinal = produtosOrdenados.Select(p => new
            {
                Id = p.id,
                Nome = p.Nome,
                Faturamento = p.ValorTotalEstoque,
                Classe = p.ValorTotalEstoque >= 100000 ? "A" : (p.ValorTotalEstoque >= 20000 ? "B" : "C")

            }).ToList();

            return Results.Ok(resultadoFinal);
        }

        public static IResult deletarProduto(int id, AppDbContext banco)
        {
            var produtoexiste = banco.Produtos.FirstOrDefault(c => c.Id == id);

            if (produtoexiste == null)
            {
                Console.WriteLine($"[{DateTime.Now}] ERRO: Produto não encontrado.");
                return Results.NotFound(new { mensagem = "Produto não encontrado." });
            }

            if (produtoexiste.QuantidadeEmEstoque > 0)
            {
                Console.WriteLine($"[{DateTime.Now}] ERRO: O produto tem itens no estoque, retire primeiro.");
                return Results.BadRequest(new { mensagem = "O produto tem itens no estoque, retire primeiro." });
            }

            banco.Produtos.Remove(produtoexiste);

            banco.SaveChanges();

            return Results.Ok("Produto removido com sucesso.");
        }

        public static IResult RelatorioLucro(AppDbContext banco)
        {
            var relatorio = banco.Produtos.Select(p => new
            {
                id = p.Id,
                nome = p.Nome,
                quantidade = p.QuantidadeEmEstoque,
                preco = p.Preco,
                precocusto = p.PrecoCusto,
                lucrototal = (p.Preco - p.PrecoCusto) * p.QuantidadeEmEstoque

            }).ToList();

            return Results.Ok(relatorio);

        }

        public static IResult ModificarDadosProduto(int id, Produto dadosNovos, AppDbContext banco)
        {

            var produtoexiste = banco.Produtos.FirstOrDefault(c => c.Id == id);

            if (produtoexiste == null)
            {
                Console.WriteLine($"[{DateTime.Now}] ERRO: Produto não encontrado.");
                return Results.NotFound(new { mensagem = "Produto não encontrado." });
            }

            if (dadosNovos == null)
            {
                Console.WriteLine($"[{DateTime.Now}] ERRO: Insira a informação que quer atualizar.");
                return Results.BadRequest(new { mensagem = "Insira a informação que quer atualizar." });
            }

            produtoexiste.Nome = dadosNovos.Nome;
            produtoexiste.CodigoDebarras = dadosNovos.CodigoDebarras;

            banco.SaveChanges();

            return Results.Ok("Informações alteradas.");

        }

        public static IResult ListarProdutosPaginados(int pagina, int tamanho, AppDbContext banco)
        {
            var produtosPaginados = banco.Produtos
                .Skip((pagina - 1) * tamanho)
                .Take(tamanho)
                .ToList();

           return Results.Ok(produtosPaginados);
        }

    }
}