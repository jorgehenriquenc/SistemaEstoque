namespace SistemaEstoque.Api.Dtos.Auth
{
    // DTO usado para cadastrar um novo usuário
    public class RegisterDto
    {
        public string Nome { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Senha { get; set; } = string.Empty;
    }
}