using SistemaEstoque.Data.Context;
using SistemaEstoque.Data.Entities;
using SistemaEstoque.Data.Repositories.Interfaces;

namespace SistemaEstoque.Data.Repositories.Implementations
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly AppDbContext _context;

        public UsuarioRepository(AppDbContext context)
        {
            _context = context;
        }

        public Usuario? BuscarPorEmail(string email)
        {
            return _context.Usuarios
                .FirstOrDefault(u => u.Email.ToLower() == email.ToLower());
        }

        public bool EmailExiste(string email)
        {
            return _context.Usuarios
                .Any(u => u.Email.ToLower() == email.ToLower());
        }

        public void Cadastrar(Usuario usuario)
        {
            _context.Usuarios.Add(usuario);
            _context.SaveChanges();
        }
    }
}