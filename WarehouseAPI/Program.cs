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

app.Run();