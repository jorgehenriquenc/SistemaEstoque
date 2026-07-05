using Microsoft.EntityFrameworkCore;
using SistemaEstoque.Data.Context;
using SistemaEstoque.Data.Entities;
using SistemaEstoque.Data.Repositories.Interfaces;

namespace SistemaEstoque.Data.Repositories.Implementations
{
    // Implementa as operações de banco de dados para Produto.
    public class ProdutoRepository : IProdutoRepository
    {
        private readonly AppDbContext _context;

        public ProdutoRepository(AppDbContext context)
        {
            _context = context;
        }

        // Retorna todos os produtos com suas categorias.
        public async Task<List<Produto>> ListarTodosAsync()
        {
            return await _context.Produtos
                .AsNoTracking()
                .Include(produto => produto.Categoria)
                .ToListAsync();
        }

        // Busca um produto pelo identificador.
        public async Task<Produto?> BuscarPorIdAsync(int id)
        {
            return await _context.Produtos
                .Include(produto => produto.Categoria)
                .FirstOrDefaultAsync(
                    produto => produto.Id == id
                );
        }

        // Busca uma categoria pelo identificador.
        public async Task<Categoria?> BuscarCategoriaPorIdAsync(
            int categoriaId)
        {
            return await _context.Categorias
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    categoria => categoria.Id == categoriaId
                );
        }

        // Cadastra um produto.
        public async Task CadastrarAsync(Produto produto)
        {
            _context.Produtos.Add(produto);

            await _context.SaveChangesAsync();
        }

        // Salva as alterações de um produto já rastreado.
        public async Task AtualizarAsync(Produto produto)
        {
            await _context.SaveChangesAsync();
        }

        // Remove um produto.
        public async Task RemoverAsync(Produto produto)
        {
            _context.Produtos.Remove(produto);

            await _context.SaveChangesAsync();
        }
    }
}