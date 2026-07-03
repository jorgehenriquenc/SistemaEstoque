using SistemaEstoque.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaEstoque.Data.Repositories.Interfaces
{
    // Define as operações de banco de dados relacionadas a pedidos.
    public interface IPedidoRepository
    {
        // Retorna todos os pedidos cadastrados.
        Task<List<Pedido>> ListarTodosAsync();

        // Busca um pedido pelo identificador.
        Task<Pedido?> BuscarPorIdAsync(int id);

        // Busca um produto pelo identificador.
        Task<Produto?> BuscarProdutoPorIdAsync(int produtoId);

        // Cadastra um novo pedido.
        Task CadastrarAsync(Pedido pedido);

        // Remove um pedido existente.
        Task RemoverAsync(Pedido pedido);

        // Retorna todos os itens de pedidos.
        Task<List<ItemPedido>> ListarItensPedidoAsync();

        // Retorna todos os produtos.
        Task<List<Produto>> ListarProdutosAsync();

        // Retorna a quantidade de categorias.
        Task<int> ContarCategoriasAsync();

        // Retorna a quantidade de pedidos.
        Task<int> ContarPedidosAsync();
    }
}