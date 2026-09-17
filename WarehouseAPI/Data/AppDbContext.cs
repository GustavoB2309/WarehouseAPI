using Microsoft.EntityFrameworkCore;
using WarehouseAPI.Models;

namespace WarehouseAPI.Data
{
    public class AppDbContext : DbContext
    {

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

    public DbSet<Produto> Produtos { get; set; }
    public DbSet<Fornecedor> Fornecedores { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }
}
}