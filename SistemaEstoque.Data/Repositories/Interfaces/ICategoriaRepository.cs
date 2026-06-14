using SistemaEstoque.Data.Entities;

namespace SistemaEstoque.Data.Repositories.Interfaces
{
    // Interface que define as operações de banco para Categoria
    public interface ICategoriaRepository
    {
        // Retorna todas as categorias cadastradas
        List<Categoria> ListarTodas();

        // Busca uma categoria pelo ID
        Categoria BuscarPorId(int id);

        // Cadastra uma nova categoria
        void Cadastrar(Categoria categoria);

        // Atualiza uma categoria existente
        void Atualizar(Categoria categoria);

        // Remove uma categoria existente
        void Remover(Categoria categoria);

        // Verifica se uma categoria possui produtos vinculados
        bool PossuiProdutos(int categoriaId);
    }
}