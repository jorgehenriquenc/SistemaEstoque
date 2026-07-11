using SistemaEstoque.Data.Entities;

namespace SistemaEstoque.Data.Repositories.Interfaces
{
    // Define as operações de banco de dados para Produto.
    public interface IProdutoRepository
    {
        // Retorna todos os produtos com suas categorias.
        Task<List<Produto>> ListarTodosAsync();

        // Busca um produto pelo identificador.
        Task<Produto?> BuscarPorIdAsync(int id);

        // Busca uma categoria pelo identificador.
        Task<Categoria?> BuscarCategoriaPorIdAsync(int categoriaId);

        // Verifica se já existe produto com o mesmo nome dentro da mesma categoria.
        Task<bool> ProdutoExisteNaCategoriaAsync(
            string nome,
            int categoriaId,
            int? idIgnorado = null
        );

        // Verifica se o produto já foi usado em algum pedido.
        Task<bool> ProdutoPossuiPedidosAsync(int produtoId);

        // Cadastra um produto.
        Task CadastrarAsync(Produto produto);

        // Salva as alterações realizadas no produto.
        Task AtualizarAsync(Produto produto);

        // Remove um produto.
        Task RemoverAsync(Produto produto);
    }
}