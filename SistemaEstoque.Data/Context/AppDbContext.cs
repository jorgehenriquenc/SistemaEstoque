using Microsoft.EntityFrameworkCore;
using SistemaEstoque.Data.Entities;

namespace SistemaEstoque.Data.Context
{
    public class AppDbContext : DbContext
    {
        // Construtor usado pela injeção de dependência da API
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Representa a tabela Produtos no banco
        public DbSet<Produto> Produtos { get; set; }

        // Representa a tabela Categorias no banco
        public DbSet<Categoria> Categorias { get; set; }

        // Representa a tabela Pedidos no banco
        public DbSet<Pedido> Pedidos { get; set; }

        // Representa a tabela ItensPedido no banco
        public DbSet<ItemPedido> ItensPedido { get; set; }
    }
}