using Microsoft.EntityFrameworkCore;
using SistemaEstoque.Data.Context;
using SistemaEstoque.Data.Entities;
using SistemaEstoque.Data.Repositories.Interfaces;

namespace SistemaEstoque.Data.Repositories.Implementations
{
    public class PedidoRepository : IPedidoRepository
    {
        private readonly AppDbContext _context;

        public PedidoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Pedido>> ListarTodosAsync()
        {
            return await _context.Pedidos
                .AsNoTracking()
                .Include(pedido => pedido.Itens)
                    .ThenInclude(item => item.Produto)
                        .ThenInclude(produto => produto.Categoria)
                .ToListAsync();
        }

        public async Task<Pedido?> BuscarPorIdAsync(int id)
        {
            return await _context.Pedidos
                .Include(pedido => pedido.Itens)
                    .ThenInclude(item => item.Produto)
                        .ThenInclude(produto => produto.Categoria)
                .FirstOrDefaultAsync(pedido => pedido.Id == id);
        }

        public async Task<Produto?> BuscarProdutoPorIdAsync(int produtoId)
        {
            return await _context.Produtos
                .FirstOrDefaultAsync(produto => produto.Id == produtoId);
        }

        public async Task CadastrarAsync(Pedido pedido)
        {
            await using var transacao = await _context.Database.BeginTransactionAsync();

            try
            {
                _context.Pedidos.Add(pedido);

                await _context.SaveChangesAsync();

                await transacao.CommitAsync();
            }
            catch
            {
                await transacao.RollbackAsync();
                throw;
            }
        }

        public async Task RemoverAsync(Pedido pedido)
        {
            await using var transacao = await _context.Database.BeginTransactionAsync();

            try
            {
                _context.Pedidos.Remove(pedido);

                await _context.SaveChangesAsync();

                await transacao.CommitAsync();
            }
            catch
            {
                await transacao.RollbackAsync();
                throw;
            }
        }

        public async Task<List<ItemPedido>> ListarItensPedidoAsync()
        {
            return await _context.ItensPedido
                .AsNoTracking()
                .Include(item => item.Produto)
                .ToListAsync();
        }

        public async Task<List<Produto>> ListarProdutosAsync()
        {
            return await _context.Produtos
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<int> ContarCategoriasAsync()
        {
            return await _context.Categorias
                .CountAsync();
        }

        public async Task<int> ContarPedidosAsync()
        {
            return await _context.Pedidos
                .CountAsync();
        }
    }
}