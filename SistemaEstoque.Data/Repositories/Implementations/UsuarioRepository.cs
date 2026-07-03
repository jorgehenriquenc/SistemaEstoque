using Microsoft.EntityFrameworkCore;
using SistemaEstoque.Data.Context;
using SistemaEstoque.Data.Entities;
using SistemaEstoque.Data.Repositories.Interfaces;

namespace SistemaEstoque.Data.Repositories.Implementations
{
    // Implementa as operações de banco de dados para usuários.
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly AppDbContext _context;

        public UsuarioRepository(AppDbContext context)
        {
            _context = context;
        }

        // Busca um usuário pelo email.
        public async Task<Usuario?> BuscarPorEmailAsync(
            string email)
        {
            return await _context.Usuarios
                .FirstOrDefaultAsync(
                    usuario => usuario.Email == email
                );
        }

        // Verifica se já existe um usuário com o email informado.
        public async Task<bool> EmailExisteAsync(string email)
        {
            return await _context.Usuarios
                .AnyAsync(
                    usuario => usuario.Email == email
                );
        }

        // Cadastra um novo usuário no banco.
        public async Task CadastrarAsync(Usuario usuario)
        {
            _context.Usuarios.Add(usuario);

            await _context.SaveChangesAsync();
        }
    }
}