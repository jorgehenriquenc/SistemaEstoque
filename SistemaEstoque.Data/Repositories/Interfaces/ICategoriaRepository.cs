using SistemaEstoque.Data.Entities;

namespace SistemaEstoque.Data.Repositories.Interfaces
{
    // Define as operações de banco de dados para Categoria.
    public interface ICategoriaRepository
    {
        // Retorna todas as categorias cadastradas.
        Task<List<Categoria>> ListarTodasAsync();

        // Busca uma categoria pelo identificador.
        Task<Categoria?> BuscarPorIdAsync(int id);

        // Verifica se já existe categoria com o mesmo nome.
        Task<bool> NomeExisteAsync(string nome, int? idIgnorado = null);

        // Cadastra uma nova categoria.
        Task CadastrarAsync(Categoria categoria);

        // Atualiza uma categoria existente.
        Task AtualizarAsync(Categoria categoria);

        // Remove uma categoria existente.
        Task RemoverAsync(Categoria categoria);

        // Verifica se existem produtos vinculados à categoria.
        Task<bool> PossuiProdutosAsync(int categoriaId);
    }
}