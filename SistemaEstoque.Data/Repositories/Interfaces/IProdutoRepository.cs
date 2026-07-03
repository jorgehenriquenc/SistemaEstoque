using SistemaEstoque.Data.Entities;

namespace SistemaEstoque.Data.Repositories.Interfaces
{
    // Define as operações de banco de dados para Produto.
    public interface IProdutoRepository
    {
        // Retorna todos os produtos cadastrados.
        Task<List<Produto>> ListarTodosAsync();

        // Busca um produto pelo identificador.
        Task<Produto?> BuscarPorIdAsync(int id);

        // Verifica se uma categoria existe.
        Task<bool> CategoriaExisteAsync(int categoriaId);

        // Cadastra um novo produto.
        Task CadastrarAsync(Produto produto);

        // Atualiza um produto existente.
        Task AtualizarAsync(Produto produto);

        // Remove um produto existente.
        Task RemoverAsync(Produto produto);
    }
}