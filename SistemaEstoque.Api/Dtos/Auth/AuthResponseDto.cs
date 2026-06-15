namespace SistemaEstoque.Api.Dtos.Auth
{
    // DTO usado para devolver o token JWT após login bem-sucedido
    public class AuthResponseDto
    {
        public string Nome { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Token { get; set; } = string.Empty;
    }
}