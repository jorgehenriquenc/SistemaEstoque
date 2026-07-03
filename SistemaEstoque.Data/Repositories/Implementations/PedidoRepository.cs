using Microsoft.EntityFrameworkCore;
using SistemaEstoque.Data.Context;
using SistemaEstoque.Data.Entities;
using SistemaEstoque.Data.Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaEstoque.Data.Repositories.Implementations
{
    // Implementa as operações de banco de dados relacionadas a pedidos.
    public class PedidoRepository : IPedidoRepository
    {
        private readonly AppDbContext _context;

        public PedidoRepository(AppDbContext context)
        {
            _context = context;
        }

        // Retorna todos os pedidos com itens, produtos e categorias.
        public async Task<List<Pedido>> ListarTodosAsync()
        {
            return await _context.Pedidos
                .Include(pedido => pedido.Itens)
                .ThenInclude(item => item.Produto)
                .ThenInclude(produto => produto.Categoria)
                .ToListAsync();
        }

        // Busca um pedido pelo ID com seus itens e produtos.
        public async Task<Pedido?> BuscarPorIdAsync(int id)
        {
            return await _context.Pedidos
                .Include(pedido => pedido.Itens)
                .ThenInclude(item => item.Produto)
                .FirstOrDefaultAsync(
                    pedido => pedido.Id == id
                );
        }

        // Busca um produto pelo ID.
        public async Task<Produto?> BuscarProdutoPorIdAsync(
            int produtoId)
        {
            return await _context.Produtos
                .FirstOrDefaultAsync(
                    produto => produto.Id == produtoId
                );
        }

        // Cadastra um novo pedido e salva as alterações de estoque.
        public async Task CadastrarAsync(Pedido pedido)
        {
            _context.Pedidos.Add(pedido);

            await _context.SaveChangesAsync();
        }

        // Remove um pedido e salva a devolução do estoque.
        public async Task RemoverAsync(Pedido pedido)
        {
            _context.Pedidos.Remove(pedido);

            await _context.SaveChangesAsync();
        }

        // Retorna todos os itens vendidos com seus produtos.
        public async Task<List<ItemPedido>> ListarItensPedidoAsync()
        {
            return await _context.ItensPedido
                .Include(item => item.Produto)
                .ToListAsync();
        }

        // Retorna todos os produtos cadastrados.
        public async Task<List<Produto>> ListarProdutosAsync()
        {
            return await _context.Produtos
                .ToListAsync();
        }

        // Retorna a quantidade total de categorias.
        public async Task<int> ContarCategoriasAsync()
        {
            return await _context.Categorias
                .CountAsync();
        }

        // Retorna a quantidade total de pedidos.
        public async Task<int> ContarPedidosAsync()
        {
            return await _context.Pedidos
                .CountAsync();
        }
    }
}