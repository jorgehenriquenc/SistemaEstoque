namespace SistemaEstoque.Api.Dtos.Auth
{
    // DTO usado para realizar login
    public class LoginDto
    {
        public string Email { get; set; } = string.Empty;

        public string Senha { get; set; } = string.Empty;
    }
}