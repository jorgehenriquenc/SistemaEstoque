using SistemaEstoque.Data.Entities;

namespace SistemaEstoque.Data.Repositories.Interfaces
{
    // Interface que define as operações de banco para Produto
    public interface IProdutoRepository
    {
        // Retorna todos os produtos cadastrados
        List<Produto> ListarTodos();

        // Busca um produto pelo ID
        Produto BuscarPorId(int id);

        // Verifica se uma categoria existe
        bool CategoriaExiste(int categoriaId);

        // Cadastra um novo produto
        void Cadastrar(Produto produto);

        // Atualiza um produto existente
        void Atualizar(Produto produto);

        // Remove um produto existente
        void Remover(Produto produto);
    }
}