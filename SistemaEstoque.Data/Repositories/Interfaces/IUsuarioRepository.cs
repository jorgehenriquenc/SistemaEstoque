using SistemaEstoque.Data.Entities;

namespace SistemaEstoque.Data.Repositories.Interfaces
{
    // Define as operações de banco de dados para usuários.
    public interface IUsuarioRepository
    {
        // Busca um usuário pelo email.
        Task<Usuario?> BuscarPorEmailAsync(string email);

        // Verifica se já existe um usuário com o email informado.
        Task<bool> EmailExisteAsync(string email);

        // Cadastra um novo usuário.
        Task CadastrarAsync(Usuario usuario);
    }
}