using Microsoft.EntityFrameworkCore;
using WarehouseAPI.Controllers;
using WarehouseAPI.Data;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
options.UseSqlServer(connectionString));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var banco = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    banco.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => "A Warehouse API está rodando.");
app.MapPost("/Abastecer", EstoqueController.AbastecerEstoque);
app.MapPost("/fornecedores/cadastrar", FornecedorController.CadastrarFornecedor);
app.MapGet("/produtos/por-fornecedor/{idFornecedor}", EstoqueController.consultarPorFornecedor);
app.MapGet("/fornecedores/{id}", FornecedorController.ConsultarPorId);
app.MapGet("/produtos", EstoqueController.listarProdutos);
app.MapDelete("/fornecedores/{id}", FornecedorController.DeletarFornecedor);
app.MapPut("/produtos/{id}/atualizar-preco", EstoqueController.AtualizarPreco);
app.MapPut("/produtos/{id}/baixa-estoque", EstoqueController.DarBaixaEstoque);
app.MapPut("/produtos/{id}/adicionar-estoque", EstoqueController.DarEntradaEstoque);
app.MapGet("/produtos/faturamento", EstoqueController.RelatorioFaturamento);
app.MapGet("/produtos/curva-abc", EstoqueController.RelatorioCurvaABC);
app.MapPut("/fornecedores/{id}/atualizar-dados", FornecedorController.AtualizarFornecedor);
app.MapDelete("/produtos/{id}", EstoqueController.deletarProduto);
app.MapGet("/produtos/lucro-total", EstoqueController.RelatorioLucro);

app.Run();