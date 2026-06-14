using SistemaEstoque.Data.Entities;

namespace SistemaEstoque.Data.Repositories.Interfaces
{
    // Interface que define as operações de banco para Pedido
    public interface IPedidoRepository
    {
        // Retorna todos os pedidos cadastrados
        List<Pedido> ListarTodos();

        // Busca um pedido pelo ID
        Pedido BuscarPorId(int id);

        // Busca um produto pelo ID
        Produto BuscarProdutoPorId(int produtoId);

        // Cadastra um novo pedido
        void Cadastrar(Pedido pedido);

        // Remove um pedido existente
        void Remover(Pedido pedido);

        // Salva alterações feitas no banco
        void SalvarAlteracoes();

        // Retorna todos os itens de pedido
        List<ItemPedido> ListarItensPedido();

        // Retorna todos os produtos
        List<Produto> ListarProdutos();

        // Retorna o total de categorias
        int ContarCategorias();

        // Retorna o total de pedidos
        int ContarPedidos();
    }
}