using SistemaEstoque.Data.Entities;

namespace SistemaEstoque.Data.Repositories.Interfaces
{
    public interface IUsuarioRepository
    {
        // Busca usuário pelo email
        Usuario? BuscarPorEmail(string email);

        // Verifica se já existe usuário com esse email
        bool EmailExiste(string email);

        // Cadastra usuário no banco
        void Cadastrar(Usuario usuario);
    }
}