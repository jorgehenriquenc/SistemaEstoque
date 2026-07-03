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

        // Retorna todos os produtos cadastrados com suas categorias.
        public async Task<List<Produto>> ListarTodosAsync()
        {
            return await _context.Produtos
                .Include(produto => produto.Categoria)
                .ToListAsync();
        }

        // Busca um produto pelo identificador com sua categoria.
        public async Task<Produto?> BuscarPorIdAsync(int id)
        {
            return await _context.Produtos
                .Include(produto => produto.Categoria)
                .FirstOrDefaultAsync(
                    produto => produto.Id == id
                );
        }

        // Verifica se a categoria existe.
        public async Task<bool> CategoriaExisteAsync(
            int categoriaId)
        {
            return await _context.Categorias
                .AnyAsync(
                    categoria => categoria.Id == categoriaId
                );
        }

        // Cadastra um produto.
        public async Task CadastrarAsync(Produto produto)
        {
            _context.Produtos.Add(produto);

            await _context.SaveChangesAsync();
        }

        // Atualiza um produto.
        public async Task AtualizarAsync(Produto produto)
        {
            _context.Produtos.Update(produto);

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