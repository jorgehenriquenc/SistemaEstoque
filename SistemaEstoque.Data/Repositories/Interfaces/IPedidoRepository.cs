using SistemaEstoque.Data.Entities;

namespace SistemaEstoque.Data.Repositories.Interfaces
{
    public interface IPedidoRepository
    {
        Task<List<Pedido>> ListarTodosAsync();

        Task<Pedido?> BuscarPorIdAsync(int id);

        Task<Produto?> BuscarProdutoPorIdAsync(int produtoId);

        Task CadastrarAsync(Pedido pedido);

        Task RemoverAsync(Pedido pedido);

        Task<List<ItemPedido>> ListarItensPedidoAsync();

        Task<List<Produto>> ListarProdutosAsync();

        Task<int> ContarCategoriasAsync();

        Task<int> ContarPedidosAsync();
    }
}