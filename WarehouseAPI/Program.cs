using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WarehouseAPI.Controllers;
using WarehouseAPI.Data;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// 1. Primeiro registramos todos os serviços da central (no topo/meio)
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("Chave_Super_Secreta_E_Gigante_Do_Galpao_2026")),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 2. 🧱 CONSTRÓI O APLICATIVO (Essa linha precisa existir e ficar aqui no meio!)
var app = builder.Build();

// 3. Depois de construído, ligamos os middlewares da portaria e as rotas
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

// (Suas rotas app.MapGet / app.MapDelete ficam aqui embaixo)
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
app.MapPatch("/produtos/{id}/modificar-dados", EstoqueController.ModificarDadosProduto);
app.MapGet("/produtos/paginar", EstoqueController.ListarProdutosPaginados);
app.MapGet("/produtos/buscar", EstoqueController.BuscarProdutoNome);
app.MapGet("/produtos/busca-por-fornecedor", EstoqueController.BuscarProdutoPorFornecedor);
app.MapPost("/usuarios/cadastrar", UsuarioController.CadastrarUsuario);
app.MapPost("/usuarios/login", UsuarioController.Login);

app.Run();