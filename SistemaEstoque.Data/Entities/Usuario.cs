namespace SistemaEstoque.Data.Entities
{
    // Representa um usuário do sistema.
    // Esse usuário poderá fazer login e receber um token JWT.
    public class Usuario
    {
        // Chave primária da tabela Usuarios
        public int Id { get; set; }

        // Nome do usuário
        public string Nome { get; set; } = string.Empty;

        // Email usado para login
        public string Email { get; set; } = string.Empty;

        // Senha protegida em formato de hash.
        // Nunca salvamos senha pura no banco.
        public string SenhaHash { get; set; } = string.Empty;
    }
}