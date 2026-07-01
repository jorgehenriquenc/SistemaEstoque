using System.ComponentModel.DataAnnotations;

namespace SistemaEstoque.Api.Dtos.Auth
{
    // DTO usado para realizar login.
    public class LoginDto
    {
        [Required(ErrorMessage = "O email é obrigatório.")]
        [EmailAddress(
            ErrorMessage = "O email informado não possui um formato válido."
        )]
        [StringLength(
            150,
            ErrorMessage = "O email deve ter no máximo 150 caracteres."
        )]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "A senha é obrigatória.")]
        [StringLength(
            100,
            MinimumLength = 6,
            ErrorMessage = "A senha deve ter entre 6 e 100 caracteres."
        )]
        public string Senha { get; set; } = string.Empty;
    }
}