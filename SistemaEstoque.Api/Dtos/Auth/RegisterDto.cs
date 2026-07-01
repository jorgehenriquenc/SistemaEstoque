using System.ComponentModel.DataAnnotations;

namespace SistemaEstoque.Api.Dtos.Auth
{
    // DTO usado para cadastrar um novo usuário.
    public class RegisterDto
    {
        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(
            100,
            MinimumLength = 2,
            ErrorMessage = "O nome deve ter entre 2 e 100 caracteres."
        )]
        public string Nome { get; set; } = string.Empty;

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